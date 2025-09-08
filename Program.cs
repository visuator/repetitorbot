using Microsoft.EntityFrameworkCore;
using repetitorbot;
using repetitorbot.Constants;
using repetitorbot.Entities.States;
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

        services.AddScoped<IQuizEngine, SimpleQuizEngine>();

        services.AddRouting(x =>
        {
            x.Command("start", x =>
            {
                x.Use<StartHandler>();
                x.Use<RenderPageHandler>();
            });

            x.Command("new", x =>
            {
                x.Use<CreateQuizHandler>();
            });

            x.Command("publish", x =>
            {
                x.Use<PublishQuizHandler>();
                x.Use<RenderPageHandler>();
            });

            x.Command("questions", x =>
            {
                x.Use<AddQuestionHandler>();
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

            x.When<StartSelectQuizState>(x =>
            {
                x.Use<SelectQuizHandler>();
                x.Use<StartQuizHandler>();
                x.Use<NextQuestionHandler>();
                x.Use<SendQuestionHandler>();
            }, (_, context) => context.Update.CallbackQuery?.Data is string s && s.StartsWith(Callback.QuizIdPrefix));

            x.When<PublishSelectQuizState>(x =>
            {
                x.Use<SetPublishedQuizHandler>();
            }, (x, context) => context.Update.CallbackQuery?.Data is string s && s.StartsWith(Callback.QuizIdPrefix));

            x.When<QuestionsSelectQuizState>(x =>
            {
                x.Use<SetQuestionsStateHandler>();
                x.Use<SendQuestionPropertyHandler>();
            }, (x, context) => context.Update.CallbackQuery?.Data is string s && s.StartsWith(Callback.QuizIdPrefix));

            x.When<AddQuestionsState>(x =>
            {
                x.Use<SetQuestionTypeHandler>();
            }, (x, context) => context.Update.CallbackQuery?.Data is string s && (s == Callback.PollQuestionType || s == Callback.TextQuestionType));

            x.When<QuizState>(x =>
            {
                x.Use<AnswerQuestionHandler>();
                x.Use<NextQuestionHandler>();
                x.Use<SendQuestionHandler>();
            });

            x.When<CreateQuizState>(x =>
            {
                x.Use<SetQuizNameHandler>();
            }, (x, _) => x.Quiz.Name is null);
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
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();