using repetitorbot.Entities.States;
using repetitorbot.Services;

namespace repetitorbot.Handlers;

internal class AnswerQuestionHandler(IQuizEngine quizEngine) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not QuizState { CurrentQuestionId: Guid currentQuestionId })
        {
            return;
        }

        if (context.Update?.Message is not { Text: string text })
        {
            return;
        }

        await quizEngine.Answer(currentQuestionId, text);

        await next(context);
    }
}
