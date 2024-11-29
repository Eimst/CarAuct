using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
{
    public Auction? GetAuction(string id)
    {
        logger.LogInformation($"Calling grpc service: {id}");
        
        var channel = GrpcChannel.ForAddress(config["GrpcAuction"] ?? throw new ArgumentException("No GrpcAuction configured"));
        var client = new GrpcAuction.GrpcAuctionClient(channel);
        var request = new GetAuctionRequest{ Id = id };

        try
        {
            var response = client.GetAuction(request);
            var auction = new Auction()
            {
                ID = response.Auction.Id,
                AuctionEnd = DateTime.Parse(response.Auction.AuctionEnd),
                Seller = response.Auction.Seller,
                ReservePrice = response.Auction.ReservePrice,
            };
            return auction;
        }
        catch (Exception e)
        {
            logger.LogError(e, " An error occured while getting auction");
            return null;
        }
    }
}