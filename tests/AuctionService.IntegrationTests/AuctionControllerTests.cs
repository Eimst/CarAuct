using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection(nameof(SharedFixture))]
public class AuctionControllerTests :  IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string GtId = "afbee524-5972-4075-8800-7d1f9d7b0a0c";
    
    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAuctions_ShouldReturnA3uctions()
    {
        // arange? 
        
        // act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("/api/auctions");
        
        // assert
        Assert.Equal(3, response?.Count);
    }
    
    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        // arange? 
        
        // act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"/api/auctions/{GtId}");
        
        // asserts
        Assert.Equal("GT", response?.Model);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInValidId_ShouldReturn404NotFound()
    {
        // arange? 
        
        // act
        var response = await _httpClient.GetAsync($"/api/auctions/{Guid.NewGuid()}");
        
        // asserts
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // arange? 
        var auction = new CreateAuctionDto
        {
            Make = "test",
            Model = "test",
            Color = "test",
            ImageUrl = "test"
        };
        
        // act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);
        
        // asserts
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // arange? 
        var auction = GetAuctionForCreate();

        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        // act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);
        
        var responseAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        
        // asserts
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("bob", responseAuction?.Seller);
    }
   
    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var auction = GetAuctionForCreate();
        auction.Make = null!;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        var updateAuctionDto = GetAuctionForUpdate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"/api/auctions/{GtId}", updateAuctionDto);
        
        // assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange
        var updateAuctionDto = GetAuctionForUpdate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("notbob"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"/api/auctions/{GtId}", updateAuctionDto);
        
        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    private CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "test",
            Color = "test",
            ImageUrl = "test",
            Mileage = 10,
            Year = 10,
            ReservePrice = 10

        };
    }
    
    private UpdateAuctionDto GetAuctionForUpdate()
    {
        return new UpdateAuctionDto
        {
            Make = "Updated",
            Model = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
        };
    }
}    