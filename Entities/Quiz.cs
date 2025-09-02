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
        public string Answer { get; set; } = null!;
    }
}
