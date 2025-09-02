using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;
using Scriban;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class QuizResponseHandler(ITelegramBotClient client, AppDbContext dbContext, CompleteQuizHandler completeQuizHandler) : Handler
{
    public override async Task Handle(Context context)
    {
        if (context.Update.Message is { Text: string } message && context.User.State is QuizState { Id: Guid quizStateId, CurrentQuestionId: Guid currentQuestionId })
        {
            var state = await dbContext.States
                .OfType<QuizState>()
                .Where(x => x.Id == quizStateId)
                .Include(x => x.LocalQuestions)
                .ThenInclude(x => x.QuizQuestion)
                .SingleAsync();

            var currentQuestion = await dbContext.LocalQuestions
                .Where(x => x.Id == currentQuestionId)
                .Include(x => x.QuizQuestion)
                .SingleAsync();

            var responseText = message.Text;
            var questionText = currentQuestion.QuizQuestion.Answer;

            var ratio = Fuzz.TokenDifferenceRatio(responseText, questionText);

            state.QuizResponses.Add(new()
            {
                LocalQuestionId = currentQuestion.Id,
                Response = responseText,
                Ratio = ratio
            });

            var (delay, pattern) = ratio switch
            {
                > 80 => (-1, "🎉 Отлично! Верно!\n\nПравильный ответ:\n\n{{correct_answer}}"),
                > 50 and < 80 => (5, "⚡ Неплохо, но попробуй точнее! Повторим через {{repeat_after}}\n\nПравильный ответ:\n\n{{correct_answer}}"),
                _ => (3, "😕 Совсем не то. Повторим через {{repeat_after}}\n\nПравильный ответ:\n\n{{correct_answer}}")
            };

            var nextQuestion = state.LocalQuestions.OrderBy(x => x.Order).FirstOrDefault(x => x.Order > currentQuestion.Order);

            currentQuestion.Order += Math.Max(0, delay);

            if (nextQuestion is null)
            {
                Next = completeQuizHandler;
                state.Completed = true;
            }
            else
            {
                state.CurrentQuestionId = nextQuestion.Id;
                var template = Template.Parse(pattern);
                var text = template.Render(new
                {
                    RepeatAfter = delay,
                    CorrectAnswer = currentQuestion.QuizQuestion.Answer
                });

                await client.SendMessage(
                    chatId: context.Update.GetChatId(),
                    text: text
                );
                await client.SendMessage(
                    chatId: context.Update.GetChatId(),
                    text: nextQuestion.QuizQuestion.Question
                );
            }
            await dbContext.SaveChangesAsync();
        }
        await base.Handle(context);
    }
}
