namespace repetitorbot.Entities.States;

internal class QuizState : State
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public Guid? CurrentQuestionId { get; set; }
    public List<LocalQuestion> LocalQuestions { get; set; } = [];
    public List<QuizResponse> QuizResponses { get; set; } = [];
    public bool Completed { get; set; }
}
internal class LocalQuestion
{
    public Guid Id { get; set; }
    public Guid QuizStateId { get; set; }
    public Guid QuizQuestionId { get; set; }
    public QuizQuestion QuizQuestion { get; set; } = null!;
    public int Order { get; set; }
}
internal class QuizResponse
{
    public Guid Id { get; set; }
    public Guid QuizStateId { get; set; }
    public QuizState QuizState { get; set; } = null!;
    public Guid LocalQuestionId { get; set; }
    public LocalQuestion LocalQuestion { get; set; } = null!;
    public string Response { get; set; } = null!;
    public int Ratio { get; set; }
}
