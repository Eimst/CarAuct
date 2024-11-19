using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions(string? date)
    {
        var query = context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();

        if (!string.IsNullOrWhiteSpace(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) >= 0);
        }

        return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        return mapper.Map<AuctionDto>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);

        // Todo: add current user as seller

        auction.Seller = User.Identity?.Name;

        await context.Auctions.AddAsync(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        if (await context.SaveChangesAsync() == 0)
            return BadRequest("Couldn't create auction");

        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
        {
            return Forbid();
        }

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var actionToPublish = mapper.Map<AuctionUpdated>(updateAuctionDto);
        actionToPublish.Id = auction.Id.ToString();
        await publishEndpoint.Publish(actionToPublish);

        if (await context.SaveChangesAsync() == 0)
        {
            return BadRequest("Couldn't update the auction");
        }

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions.FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        if (auction.Seller != User.Identity?.Name)
        {
            return Forbid();
        }
        
        context.Auctions.Remove(auction);

        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id.ToString() });

        if (await context.SaveChangesAsync() == 0)
        {
            return BadRequest("Couldn't delete auction");
        }

        return NoContent();
    }
}