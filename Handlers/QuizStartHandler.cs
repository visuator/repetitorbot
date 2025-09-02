using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class QuizStartHandler(ITelegramBotClient client, AppDbContext dbContext) : Handler
{
    public override async Task Handle(Context context)
    {
        if (context.User.State is QuizState state)
        {
            var questions = await dbContext.Quizes
                .Where(x => x.Id == state.QuizId)
                .Include(x => x.Questions)
                .Select(x => x.Questions)
                .SingleAsync();

            var localQuestions = questions.Select((x, i) => new LocalQuestion() { QuizQuestionId = x.Id, QuizStateId = state.Id, Order = i }).ToList();

            await dbContext.LocalQuestions.AddRangeAsync(localQuestions);
        
            var currentQuestion = localQuestions.First();

            state.CurrentQuestionId = currentQuestion.Id;

            await dbContext.SaveChangesAsync();

            await client.SendMessage(
                chatId: context.Update.GetChatId(),
                text: currentQuestion.QuizQuestion.Question
            );
        }
        await base.Handle(context);
    }
}
