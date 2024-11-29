using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;

namespace BiddingService.Services;

public class CheckAuctionFinished(ILogger<CheckAuctionFinished> logger,
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting check for auction finished");
        
        stoppingToken.Register(() => logger.LogInformation("Stopping check for auction finished"));

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);
            
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        var finishedAuction = await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => x.Finished == false)
            .ExecuteAsync(stoppingToken);
        
        if (finishedAuction.Count == 0)
        {
            return;
        }
       
        logger.LogInformation($"Found finished actions: {finishedAuction.Count}");
        
        using var scope = serviceProvider.CreateScope();
        
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finishedAuction)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winningBid = await DB.Find<Bid>()
                .Match(x => x.AuctionId == auction.ID)
                .Match(x => x.BidStatus == BidStatus.Accepted)
                .Sort(x => x.Descending(a => a.Amount))
                .ExecuteFirstAsync(stoppingToken);
            
            await endpoint.Publish(new AuctionFinished
            {
                ItemSold = winningBid != null,
                AuctionId = auction.ID,
                Winner = winningBid?.Bidder,
                Amount = winningBid?.Amount ?? 0,
                Seller = auction.Seller!,
            }, stoppingToken);
        }
    }
}