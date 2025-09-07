using System.Text.Json;
using System.Text.Json.Serialization;
using repetitorbot.Entities;
using repetitorbot.Services.Common;
using Telegram.Bot;

namespace repetitorbot.Handlers;

[JsonDerivedType(typeof(TextQuizQuestionDto), 0)]
[JsonDerivedType(typeof(PollQuizQuestionDto), 1)]
internal class QuizQuestionDto
{
    public string Question { get; set; } = null!;
}
internal class TextQuizQuestionDto : QuizQuestionDto
{
    public string Answer { get; set; } = null!;
}
internal class PollQuizQuestionDto : QuizQuestionDto
{
    public List<PollQuizQuestionVariantDto> Variants { get; set; } = [];
}
internal class PollQuizQuestionVariantDto
{
    public string Value { get; set; } = null!;
    public bool IsCorrect { get; set; }
}
internal class QuizDto
{
    public string Name { get; set; } = null!;
    public List<QuizQuestionDto> Questions { get; set; } = [];
}
internal class ImportQuizHandler(ITelegramBotClient client, TelegramFileService telegramFileService, AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.Update.Message is not { Id: int messageId, Document.FileId: string fileId })
        {
            return;
        }

        QuizDto dto;
        using (MemoryStream ms = new())
        {
            await telegramFileService.Download(fileId, ms);
            dto = JsonSerializer.Deserialize<QuizDto>(ms) ?? throw new InvalidOperationException("invalid json quiz format");
        }

        await dbContext.Quizes.AddAsync(new()
        {
            Name = dto.Name,
            Questions = [.. dto.Questions.Select<QuizQuestionDto, QuizQuestion>(x => x switch {
                TextQuizQuestionDto text => new TextQuizQuestion(){
                    Question = text.Question,
                    Answer = text.Answer
                },
                PollQuizQuestionDto poll => new PollQuizQuestion(){
                    Question = poll.Question,
                    Variants = [.. poll.Variants.Select(x => new PollQuestionVariant() { IsCorrect = x.IsCorrect, Value = x.Value })]
                },
                _ => throw new InvalidOperationException("not supported")
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

        await next(context);
    }
}
