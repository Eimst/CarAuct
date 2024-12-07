using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
    [Fact]
    public void HasReservePrice_ReservePriceGtZero_True()
    {
        // arange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 10,
            Item = null!
        };
        
        // action
        var result = auction.HasReservePrice();
        
        // assert
        
        Assert.True(result);
    }
    
    [Fact]
    public void HasReservePrice_ReservePriceIsZero_False()
    {
        // arange
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = 0,
            Item = null!
        };
        
        // action
        var result = auction.HasReservePrice();
        
        // assert
        
        Assert.False(result);
    }
}
