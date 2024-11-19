using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Bid Placed--");

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if (auction == null)
        {
            return;
        }

        if (context.Message.Amount > auction.CurrentHigh || context.Message.BidStatus.Contains("Accepted"))
        {
            auction.CurrentHigh = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}