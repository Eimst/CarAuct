using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext dbContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await dbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction == null)
        {
            return;
        }

        if (context.Message.Amount > (auction.CurrentHigh ?? 0) && context.Message.BidStatus.Contains("Accepted"))
        {
            auction.CurrentHigh = context.Message.Amount;
        }
    }
}