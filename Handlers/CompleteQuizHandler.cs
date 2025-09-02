using repetitorbot.Entities.States;
using Scriban;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class CompleteQuizHandler(ITelegramBotClient client) : Handler
{
    public override async Task Handle(Context context)
    {
        if (context.User.State is QuizState state)
        {
            var responses = state.QuizResponses;

            var avg = responses.Sum(x => x.Ratio) / (double)responses.Count;

            var pattern = avg switch
            {
                > 90 => "🎉 Ты закончил опросник! Результат впечатляет: {{percent}}% верных ответов. Отличная работа!",
                > 50 and < 90 => "👍 Ты завершил опросник! У тебя {{percent}}% правильных — неплохо, но можно ещё лучше 😉",
                _ => "😕 Ты дошёл до конца, но результат пока скромный: {{percent}}%. Попробуешь ещё раз?"
            };

            var template = Template.Parse(pattern);
            var text = template.Render(new { Percent = avg.ToString() });

            await client.SendMessage(
                chatId: context.Update.GetChatId(),
                text: text
            );
        }
        await base.Handle(context);
    }
}
