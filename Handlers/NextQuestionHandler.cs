using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

internal class NextQuestionHandler(AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not QuizState state)
        {
            return;
        }

        var currentQuestion = await dbContext.UserQuizQuestions
            .SingleOrDefaultAsync(x => x.Id == state.CurrentQuestionId);
        var question = await dbContext.UserQuizQuestions
            .Where(x => x.QuizStateId == state.Id)
            .Where(x => x.Order > (currentQuestion == null ? 0 : currentQuestion.Order))
            .OrderBy(x => x.Order)
            .FirstOrDefaultAsync();

        state.CurrentQuestionId = question?.Id;

        await next(context);
    }
}
