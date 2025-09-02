using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;
using repetitorbot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using User = repetitorbot.Entities.User;

namespace repetitorbot
{
    internal class UpdateHandler(CommandRouter commandRouter, TextRouter textRouter, CallbackQueryRouter callbackQueryRouter, FileRouter fileRouter, AppDbContext dbContext) : IUpdateHandler
    {
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message)
            {
                var user = await EnsureUser(message.GetUserId());

                if (message is { Document.FileId: string fileId })
                {
                    await fileRouter.Route(user, update, fileId, cancellationToken);
                }

                if (message is { Text: string text })
                {
                    if (text.StartsWith('/'))
                    {
                        await commandRouter.Route(user, update, cancellationToken);
                    }
                    else
                    {
                        await textRouter.Route(user, update, cancellationToken);
                    }   
                }
            }
            else if (update.CallbackQuery is CallbackQuery query)
            {
                var user = await EnsureUser(query.GetUserId());

                await callbackQueryRouter.Route(user, update, cancellationToken);
            }
        }

        private async Task<User> EnsureUser(long userId)
        {
            var user = await dbContext.Users
                .Where(x => x.Id == userId)
                .Include(x => x.State)
                .SingleOrDefaultAsync();
            if (user is null)
            {
                user = new User { Id = userId };
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            return user;
        }
    }
    internal class FileRouter(ITelegramBotClient client, ImportQuizHandler importQuizHandler)
    {
        public async Task Route(User user, Update update, string fileId, CancellationToken cancellationToken)
        {
            var file = await client.GetFile(fileId, cancellationToken);
            if (file is { FilePath: string path })
            {
                if (Path.GetExtension(path) is ".json")
                {
                    await importQuizHandler.Handle(new Context(update, user));
                }
            }
        }
    }
    internal class CallbackQueryRouter(Func<State, Handler> factory)
    {
        public async Task Route(User user, Update update, CancellationToken cancellationToken)
        {
            if (user.State is not null)
            {
                var handler = factory(user.State);
                await handler.Handle(new Context(update, user));
            }
        }
    }
    internal class TextRouter(Func<State, Handler> factory)
    {
        public async Task Route(User user, Update update, CancellationToken cancellationToken)
        {
            if (user.State is not null)
            {
                var handler = factory(user.State);
                await handler.Handle(new Context(update, user));
            }
        }
    }
    internal class CommandRouter([FromKeyedServices("start")] Handler startHandler)
    {
        public async Task Route(User user, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { Text: string text } message)
            {
                if (text.StartsWith("/start"))
                {
                    await startHandler.Handle(new Context(update, user));   
                }
            }   
        }
    }
}
