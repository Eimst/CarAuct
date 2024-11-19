using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer(IMapper mapper) : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("------> Consuming actionupdated: " + context.Message.Id);

        var item = await DB.Find<Item>()
            .Match(i => i.ID == context.Message.Id)
            .ExecuteSingleAsync();

        if (item != null)
        {
            item.Color = context.Message.Color ?? item.Color;
            item.Make = context.Message.Make ?? item.Make;
            item.Model = context.Message.Model ?? item.Model;
            item.Mileage = context.Message.Mileage;
            item.Year = context.Message.Year;

            try
            {
                await DB.SaveAsync(item);
            }
            catch (Exception ex)
            {
                throw new MessageException(typeof(AuctionUpdated), "Problem saving changes", ex);
            }
        }
    }
}