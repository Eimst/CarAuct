using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer(AuctionDbContext contextDb) : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        var auction = await contextDb.Auctions.FirstOrDefaultAsync(x => Equals(x.Id, context.Message.AuctionId));

        if (auction == null)
        {
            return;
        }
        
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status = auction.SoldAmount >= auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;
        
        await contextDb.SaveChangesAsync();
    }
}