namespace repetitorbot.Entities.States;

internal class AddQuestionsState : State
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public AddQuestionsProperty CurrentProperty { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Value { get; set; } = null!;
    public int LastMessageId { get; set; }
}
internal enum AddQuestionsProperty
{
    None,
    Question,
    TextAnswer,
    QuestionType
}
internal enum QuestionType
{
    None,
    Poll,
    Text
}
