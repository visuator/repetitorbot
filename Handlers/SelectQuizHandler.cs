using repetitorbot.Entities.States;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class SelectQuizHandler(ITelegramBotClient client) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.Update.CallbackQuery is not { Data: string data })
        {
            return;
        }

        if (!Guid.TryParse(data.AsSpan()["quizId:".Length..], out var quizId))
        {
            return;
        }

        if (context.State is not SelectQuizState { MessageId: int messageId })
        {
            return;
        }

        context.State = new QuizState()
        {
            QuizId = quizId
        };

        await client.DeleteMessage(
            chatId: context.Update.GetChatId(),
            messageId: messageId
        );

        await next(context);
    }
}
