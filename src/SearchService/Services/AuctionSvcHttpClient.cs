using System.Globalization;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
{
    public async Task<List<Item>?> GetItemForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(i => i.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString("o"))
            .ExecuteFirstAsync();

        return await httpClient.GetFromJsonAsync<List<Item>>(config["AuctionServiceUrl"] +
                                                             $"/api/auctions?date={lastUpdated}");
    }
}