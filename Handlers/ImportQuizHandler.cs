using System.Text.Json;
using repetitorbot.Entities;
using Telegram.Bot;

namespace repetitorbot.Handlers;

internal class QuizQuestionDto
{
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
}
internal class QuizDto
{
    public string Name { get; set; } = null!;
    public List<QuizQuestionDto> Questions { get; set; } = [];
}
internal class ImportQuizHandler(ITelegramBotClient client, AppDbContext dbContext) : Handler
{
    public override async Task Handle(Context context)
    {
        if (context.Update.Message is { Id: int messageId, Document.FileId: string fileId })
        {
            var file = await client.GetFile(fileId);
            using var ms = new MemoryStream();
            await client.DownloadFile(file, ms);
            ms.Position = 0;

            var dto = JsonSerializer.Deserialize<QuizDto>(ms) ?? throw new InvalidOperationException("unknown json object");
            await dbContext.Quizes.AddAsync(new()
            {
                Name = dto.Name,
                Questions = [.. dto.Questions.Select(x => new QuizQuestion()
                {
                    Question = x.Question,
                    Answer = x.Answer
                })]
            });
            await dbContext.SaveChangesAsync();
            await client.DeleteMessage(
                chatId: context.Update.GetChatId(),
                messageId: messageId
            );
            await client.SendMessage(
                chatId: context.Update.GetChatId(),
                text: "✅ Квиз успешно импортирован!"
            );
        }

        await base.Handle(context);
    }
}
