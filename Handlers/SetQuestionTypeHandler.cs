
using repetitorbot.Entities.States;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class SetQuestionTypeHandler(ITelegramBotClient client) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.Update.CallbackQuery is not { Data: string data })
        {
            return;
        }

        if (!Enum.TryParse<QuestionType>(data, out var type))
        {
            return;
        }

        if (context.State is not AddQuestionsState state)
        {
            return;
        }

        state.QuestionType = type;

        await client.DeleteMessage(
            chatId: context.Update.GetChatId(),
            messageId: state.LastMessageId
        );
        await client.SendMessage(
            chatId: context.Update.GetChatId(),
            text: "Тип вопроса изменен"
        );

        await next(context);
    }
}
