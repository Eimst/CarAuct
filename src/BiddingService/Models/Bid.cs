using MongoDB.Entities;

namespace BiddingService.Models;

public class Bid : Entity
{
    public required string AuctionId { get; set; }
    
    public required string Bidder { get; set; }

    public required DateTime BidTime { get; set; } = DateTime.UtcNow;

    public required int Amount { get; set; }

    public BidStatus BidStatus { get; set; }
}