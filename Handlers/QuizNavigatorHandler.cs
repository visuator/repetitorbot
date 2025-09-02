using repetitorbot.Entities.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace repetitorbot.Handlers;

internal class QuizNavigatorHandler(ITelegramBotClient client, AppDbContext dbContext, QuizStartHandler quizStartHandler, RenderPageHandler renderPageHandler) : Handler
{
    public override async Task Handle(Context context)
    {
        var user = context.User;
        if (user.State is SelectQuizState state)
        {
            if (context.Update.CallbackQuery is CallbackQuery { Data: string } query)
            {
                if (Guid.TryParse(query.Data, out var quizId))
                {
                    state.QuizId = quizId;

                    QuizState quizState = new()
                    {
                        QuizId = quizId
                    };
                    user.State = quizState;
                    await dbContext.SaveChangesAsync();

                    if (state.MessageId is int messageId)
                    {
                        await client.DeleteMessage(
                            chatId: context.Update.GetChatId(),
                            messageId: messageId
                        );
                    }
                    Next = quizStartHandler;
                }
                else
                {
                    var previous = state.CurrentPage;
                    switch (query.Data)
                    {
                        case "forward":
                            if (state.CurrentPage + 1 < state.PagesCount)
                                state.CurrentPage++;
                            break;
                        case "back":
                            if (state.CurrentPage - 1 > 0)
                                state.CurrentPage--;
                            break;
                    }
                    if (state.CurrentPage != previous)
                    {
                        await dbContext.SaveChangesAsync();
                        Next = renderPageHandler;
                    }
                    await client.AnswerCallbackQuery(query.Id);
                }
            }
        }
        await base.Handle(context);
    }
}
