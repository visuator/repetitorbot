using Microsoft.EntityFrameworkCore;
using repetitorbot;
using repetitorbot.Entities.States;
using repetitorbot.Handlers;
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

        services.AddScoped<CommandRouter>();
        services.AddScoped<TextRouter>();
        services.AddScoped<CallbackQueryRouter>();
        services.AddScoped<FileRouter>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(context.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("empty database connection string"));
        }, ServiceLifetime.Scoped);

        services.AddScoped<StartHandler>();
        services.AddScoped<QuizNavigatorHandler>();
        services.AddScoped<RenderPageHandler>();
        services.AddScoped<QuizStartHandler>();
        services.AddScoped<QuizResponseHandler>();
        services.AddScoped<CompleteQuizHandler>();
        services.AddScoped<ImportQuizHandler>();

        services.AddTransient<PipelineBuilder>();
        services.AddScoped<Func<State, Handler>>(provider =>
            state =>
            {
                var pipelineBuilder = provider.GetRequiredService<PipelineBuilder>();
                return state switch
                {
                    QuizState quizState when !quizState.Completed =>
                        pipelineBuilder
                            .Use<QuizResponseHandler>()
                            .Result,
                    SelectQuizState =>
                        pipelineBuilder
                            .Use<QuizNavigatorHandler>()
                            .Result,
                    _ => throw new InvalidOperationException("unknown state")
                };
            }
        );
        services.AddKeyedScoped("start", (provider, _) =>
        {
            var pipelineBuilder = provider.GetRequiredService<PipelineBuilder>();
            return pipelineBuilder
                .Use<StartHandler>()
                .Use<RenderPageHandler>()
                .Result;
        });
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();