using MongoDB.Entities;

namespace BiddingService.Models;

public class Auction : Entity
{
    public required DateTime AuctionEnd { get; set; }

    public string? Seller { get; set; }

    public required int ReservePrice { get; set; }

    public bool? Finished { get; set; }
}