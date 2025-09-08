using repetitorbot.Entities.States;

namespace repetitorbot.Services;

internal interface IQuizEngine
{
    Task Answer(Guid userQuizQuestionId, string text);
}
internal class SimpleQuizEngine(AppDbContext dbContext) : IQuizEngine
{
    public async Task Answer(Guid userQuizQuestionId, string text)
    {
        QuizQuestionRespone response = new()
        {
            Text = text,
            UserQuizQuestionId = userQuizQuestionId
        };

        await dbContext.QuizQuestionResponses.AddAsync(response);
        await dbContext.SaveChangesAsync();
    }
}
