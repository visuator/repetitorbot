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
    }
}
