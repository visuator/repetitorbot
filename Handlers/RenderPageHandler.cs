using repetitorbot.Entities.States;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace repetitorbot.Handlers;

internal class RenderPageHandler(ITelegramBotClient client, AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not SelectQuizState state)
        {
            return;
        }

        var quizes = dbContext.Quizes
            .Skip(state.ItemsPerPage * (state.CurrentPage - 1))
            .Take(state.ItemsPerPage)
            .OrderBy(x => x.Name)
            .ToList();

        if (quizes.Count == 0)
        {
            await client.SendMessage(
                chatId: context.Update.GetChatId(),
                text: "📭 Сейчас нет доступных опросников."
            );
            context.State = null;
        }
        else
        {
            var keyboard = new InlineKeyboardMarkup(
                inlineKeyboard: quizes
                    .Select(x => new InlineKeyboardButton(x.Name, $"quizId:{x.Id}"))
                    .Chunk(3)
                    .Append([
                        new InlineKeyboardButton("←", "back"),
                        new InlineKeyboardButton("→", "forward")
                    ])
            );

            if (state.MessageId is not int messageId)
            {
                var message = await client.SendMessage(
                    chatId: context.Update.GetChatId(),
                    replyMarkup: keyboard,
                    text: "🔎 Найдено несколько опросников. Выбери нужный:"
                );
                state.MessageId = message.Id;
            }
            else
            {
                await client.EditMessageReplyMarkup(
                    chatId: context.Update.GetChatId(),
                    messageId: messageId,
                    replyMarkup: keyboard
                );
            }
        }
        await next(context);
    }
}
