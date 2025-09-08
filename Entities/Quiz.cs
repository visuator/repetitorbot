namespace repetitorbot.Entities
{
    internal class Quiz
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<QuizQuestion> Questions { get; set; } = [];
    }
    internal class QuizQuestion
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public string Question { get; set; } = null!;
        public int Order { get; set; }
    }
    internal class TextQuizQuestion : QuizQuestion
    {
        public string Answer { get; set; } = null!;
    }
    internal class PollQuizQuestion : QuizQuestion
    {
        public List<PollQuestionVariant> Variants { get; set; } = [];
    }
    internal class PollQuestionVariant
    {
        public Guid Id { get; set; }
        public Guid PollQuizQuestionId { get; set; }
        public PollQuizQuestion PollQuizQuestion { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public string Value { get; set; } = null!;
    }
}
