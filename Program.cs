using Microsoft.EntityFrameworkCore;
using repetitorbot;
using repetitorbot.Constants;
using repetitorbot.Extensions;
using repetitorbot.Handlers;
using repetitorbot.Middlewares;
using repetitorbot.Services;
using repetitorbot.Services.Common;
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

        services.AddSingleton<TelegramFileService>();

        services.AddRouting(x =>
        {
            x.Command("start", x =>
            {
                x.Use<StartHandler>();
                x.Use<RenderPageHandler>();
            });

            x.File("json", x =>
            {
                x.Use<ImportQuizHandler>();
            });

            x.Callback(x => x is Callback.ForwardPage, x =>
            {
                x.Use<ForwardPageHandler>();
                x.Use<RenderPageHandler>();
            });

            x.Callback(x => x is Callback.BackPage, x =>
            {
                x.Use<BackPageHandler>();
                x.Use<RenderPageHandler>();
            });

            x.Callback(x => x.StartsWith(Callback.QuizIdPrefix), x =>
            {
                x.Use<SelectQuizHandler>();
            });
        });

        services.AddPipeline(x =>
        {
            x.Use<EnsureUserMiddleware>();
            x.Use<SetStateMiddleware>();
            x.Use<RouterMiddleware>();
            x.Use<SaveStateMiddleware>();
            x.Use<AnswerCallbackQueryMiddleware>();
        });
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();