using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController(IMapper mapper, IPublishEndpoint endpoint, GrpcAuctionClient grpcAuctionClient) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        if (auction == null)
        {
            auction = grpcAuctionClient.GetAuction(auctionId);
            
            if (auction == null)
                return BadRequest("Cannot accept bids on this auction");
        }

        if (auction.Seller == User.Identity?.Name)
        {
            return BadRequest("You cannot place bid on your own auction");
        }

        var bid = new Bid
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity?.Name!,
            BidTime = DateTime.UtcNow,
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
            return Ok(mapper.Map<BidDto>(bid));
        }

        var highBid = await DB.Find<Bid>().Match(a => a.AuctionId == auctionId && a.Amount > amount)
            .ExecuteFirstAsync();

        if (highBid == null)
        {
            bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
        }
        else
        {
            bid.BidStatus = BidStatus.TooLow;
        }

        await DB.SaveAsync(bid);
        
        await endpoint.Publish(mapper.Map<BidPlaced>(bid));

        return Ok(mapper.Map<BidDto>(bid));
    }


    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDto>>> GetBidsForAuction(string auctionId)
    {
        var bids = await DB.Find<Bid>().Match(a => a.AuctionId == auctionId)
            .Sort(b => b.Descending(a => a.BidTime)).ExecuteAsync();
        
        return bids.Select(mapper.Map<BidDto>).ToList();
    }
}