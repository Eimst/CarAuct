using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepository;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly AuctionController _auctionController;
    private readonly IMapper _mapper;
    
    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepository = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;
        
        _mapper = new Mapper(mockMapper);
        
        _auctionController = new AuctionController(_auctionRepository.Object, _mapper, _publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };
    }

    [Fact]
    public async Task GetAuctions_WithNoParams_Returns10Auctions()
    {
        // arange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        
        _auctionRepository.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);
        
        // act 
        var result = await _auctionController.GetAllAuctions(null);
        
        // assert
        Assert.Equal(10, result.Value?.Count());
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }
    
    [Fact]
    public async Task GetAuctionsById_WithValidGuid_ReturnsAuction()
    {
        // arange
        var auction = _fixture.Create<AuctionDto>();
        
        _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
        
        // act 
        var result = await _auctionController.GetAuctionById(auction.Id);
        
        // assert
        Assert.Equal(auction.Make, result.Value?.Make);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }
    
    [Fact]
    public async Task GetAuctionsById_WithInvalidGuid_ReturnsNotFound()
    {
        // arange
        
        _auctionRepository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);
        
        // act 
        var result = await _auctionController.GetAuctionById(Guid.NewGuid());
        
        // assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
    {
        // arange
        var auction = _fixture.Create<CreateAuctionDto>();
        
        _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        
        // act 
        var result = await _auctionController.CreateAuction(auction);
        var createdResult = result.Result as CreatedAtActionResult;
        // assert
        Assert.NotNull(createdResult);
        Assert.Equal("GetAuctionById", createdResult.ActionName);
        Assert.IsType<AuctionDto>(createdResult.Value);
    }
    
    [Fact]
    public async Task CreateAuction_WithDatabaseError_ReturnsBadRequest()
    {
        // arange
        var auction = _fixture.Create<CreateAuctionDto>();
        
        _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
        
        // act 
        var result = await _auctionController.CreateAuction(auction);
        // assert
        Assert.Null(result.Value);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithValidGuidAndValidUpdateAuctionDto_ReturnsNoContent()
    {
        // arange
        var auction = _fixture.Create<UpdateAuctionDto>();
        
        var updateAuction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        updateAuction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        updateAuction.Seller = "test";
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(updateAuction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        
        // act 
        var result = await _auctionController.UpdateAuction(It.IsAny<Guid>(), auction);
        
        // assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithNotMatchingUser_ReturnsForbid()
    {
        // arange
        var auction = _fixture.Create<UpdateAuctionDto>();
        
        var updateAuction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        updateAuction.Seller = "not-test";
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(updateAuction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        
        // act 
        var result = await _auctionController.UpdateAuction(It.IsAny<Guid>(), auction);
        
        // assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_DatabaseError_ReturnsBadRequest()
    {
        // arange
        var updateAuction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        updateAuction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        updateAuction.Seller = "test";
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(updateAuction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
        
        // act 
        var result = await _auctionController.DeleteAuction(It.IsAny<Guid>());
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
    {
        // arange
        var auction = _fixture.Create<UpdateAuctionDto>();
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);
        
        // act 
        var result = await _auctionController.UpdateAuction(It.IsAny<Guid>(), auction);
        
        // assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithValidGuid_ReturnsNoContent()
    {
        // arange
        var deleteAuction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        deleteAuction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        deleteAuction.Seller = "test";
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(deleteAuction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        
        // act 
        var result = await _auctionController.DeleteAuction(It.IsAny<Guid>());
        
        // assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_DatabaseError_ReturnsBadRequest()
    {
        // arange
        var deleteAuction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        deleteAuction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        deleteAuction.Seller = "test";
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(deleteAuction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
        
        // act 
        var result = await _auctionController.DeleteAuction(It.IsAny<Guid>());
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithNotMatchingUser_ReturnsForbid()
    {
        // arange
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Seller = "not-test";
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(auction);
        
        _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
        
        // act 
        var result = await _auctionController.DeleteAuction(It.IsAny<Guid>());
        
        // assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithInvalidGuid_ReturnsNotFound()
    {
        // arange
        
        _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);
        
        // act 
        var result = await _auctionController.DeleteAuction(It.IsAny<Guid>());
        
        // assert
        Assert.IsType<NotFoundResult>(result);
    }
}