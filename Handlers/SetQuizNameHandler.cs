
using repetitorbot.Entities.States;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class SetQuizNameHandler(ITelegramBotClient client) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not CreateQuizState state)
        {
            return;
        }

        if (context.Update.Message is not { Text: string text })
        {
            return;
        }

        state.Quiz.Name = text;

        await client.SendMessage(
            chatId: context.Update.GetChatId(),
            text: "Имя успешно установлено"
        );

        await next(context);
    }
}
