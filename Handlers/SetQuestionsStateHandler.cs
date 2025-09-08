
using repetitorbot.Constants;
using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

internal class SetQuestionsStateHandler : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.Update.CallbackQuery is not { Data: string data })
        {
            return;
        }

        if (!Guid.TryParse(data.AsSpan()[Callback.QuizIdPrefix.Length..], out var quizId))
        {
            return;
        }

        context.State = new AddQuestionsState()
        {
            UserId = context.User.Id,
            CurrentProperty = AddQuestionsProperty.QuestionType,
            QuestionType = QuestionType.None,
            QuizId = quizId
        };

        await next(context);
    }
}
