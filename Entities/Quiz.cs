namespace repetitorbot.Entities
{
    internal class Quiz
    {
        public Guid Id { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
        public string? Name { get; set; } = null!;
        public bool Published { get; set; }
        public QuizType Type { get; set; }
        public List<QuizQuestion> Questions { get; set; } = [];
    }
    internal enum QuizType
    {
        Public,
        Private
    }
    internal class QuizQuestion
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public string Question { get; set; } = null!;
        public int Order { get; set; }
        public MatchAlgorithm MatchAlgorithm { get; set; }
        public List<QuizQuestionCategoryLink> Categories { get; set; } = [];
    }
    internal class QuizQuestionCategoryLink
    {
        public Guid QuizQuestionId { get; set; }
        public QuizQuestion QuizQuestion { get; set; } = null!;
        public Guid QuizQuestionCategoryId { get; set; }
        public QuizQuestionCategory QuizQuestionCategory { get; set; } = null!;
    }
    internal class QuizQuestionCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<QuizQuestionCategoryLink> Questions { get; set; } = [];
    }
    internal enum MatchAlgorithm
    {
        Fuzzy,
        Exact
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
