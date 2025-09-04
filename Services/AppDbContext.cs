using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities;
using repetitorbot.Entities.States;

namespace repetitorbot
{
    internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Quiz> Quizes { get; set; } = null!;
        public DbSet<State> States { get; set; } = null!;
        public DbSet<LocalQuestion> LocalQuestions { get; set; } = null!;
        public DbSet<QuizResponse> QuizResponses { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>()
                .HasDiscriminator<string>("Type")
                .HasValue<QuizState>("QuizStates")
                .HasValue<SelectQuizState>("SelectQuizStates");
            base.OnModelCreating(modelBuilder);
        }
    }
}
