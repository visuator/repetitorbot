using Microsoft.EntityFrameworkCore;
using repetitorbot;
using repetitorbot.Extensions;
using repetitorbot.Handlers;
using repetitorbot.Middlewares;
using repetitorbot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient("TelegramBotClient")
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, _) => new TelegramBotClient(context.Configuration.GetConnectionString("TelegramBotToken") ?? throw new InvalidOperationException("empty bot token"), httpClient));
        services.AddHostedService<PollingService>();

        services.AddScoped<IUpdateHandler, UpdateHandler>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(context.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("empty database connection string"));
        }, ServiceLifetime.Scoped);

        services.AddRouting(x =>
        {
            x.Command<StartHandler>("start");
        });

        services.AddPipeline(x =>
        {
            x.UseMiddleware<UserMiddleware>();
            x.UseMiddleware<StateMiddleware>();
        });
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();