using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class RenderQuizQuestionHandler(ITelegramBotClient client, AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not QuizState { CurrentQuestionId: Guid currentQuestionId } state)
        {
            await client.SendMessage(
                chatId: context.Update.GetChatId(),
                text: "Вы завершили опросник"
            );
            return;
        }

        var currentQustion = await dbContext.UserQuizQuestions
            .Include(x => x.QuizQuestion)
            .SingleAsync(x => x.Id == currentQuestionId);

        await client.SendMessage(
            chatId: context.Update.GetChatId(),
            text: currentQustion.QuizQuestion.Question
        );
        
        await next(context);
    }
}
