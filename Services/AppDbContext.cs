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
        public DbSet<UserQuizQuestion> UserQuizQuestions { get; set; } = null!;
        public DbSet<QuizQuestionRespone> QuizQuestionResponses { get; set; } = null!;
        public DbSet<QuizQuestionCategory> QuizQuestionCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>()
                .HasDiscriminator<string>("Type")
                .HasValue<QuizState>("QuizState")
                .HasValue<CreateQuizState>("CreateQuizState")
                .HasValue<PublishSelectQuizState>("PublishSelectQuizState")
                .HasValue<StartSelectQuizState>("StartSelectQuizState")
                .HasValue<QuestionsSelectQuizState>("QuestionsSelectQuizState");
            modelBuilder.Entity<User>()
                .HasOne(x => x.State)
                .WithOne(x => x.User)
                .HasForeignKey<State>(x => x.UserId);

            modelBuilder.Entity<QuizQuestionCategoryLink>()
                .HasKey(x => new { x.QuizQuestionId, x.QuizQuestionCategoryId });

            modelBuilder.Entity<Quiz>()
                .HasData([
                    new() { Id = Guid.NewGuid(), Name = "test1", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test2", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test3", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test4", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test5", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test6", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test7", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test8", Published = true },
                    new() { Id = Guid.NewGuid(), Name = "test9" },
                ]);

            base.OnModelCreating(modelBuilder);
        }
    }
}
