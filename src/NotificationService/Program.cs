using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));
   
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest") ?? "guest");
            host.Password(builder.Configuration.GetValue(key: "RabbitMQ:Password", defaultValue: "guest") ?? "guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();
