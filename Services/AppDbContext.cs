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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>()
                .HasDiscriminator<string>("Type")
                .HasValue<QuizState>("QuizStates")
                .HasValue<SelectQuizState>("SelectQuizStates");
            modelBuilder.Entity<User>()
                .HasOne(x => x.State)
                .WithOne(x => x.User)
                .HasForeignKey<State>(x => x.UserId);

            modelBuilder.Entity<QuizQuestionRespone>()
                .HasKey(x => new { x.UserQuizQuestionId });

            modelBuilder.Entity<Quiz>()
                .HasData([
                    new() { Id = Guid.NewGuid(), Name = "test1" },
                    new() { Id = Guid.NewGuid(), Name = "test2" },
                    new() { Id = Guid.NewGuid(), Name = "test3" },
                    new() { Id = Guid.NewGuid(), Name = "test4" },
                    new() { Id = Guid.NewGuid(), Name = "test5" },
                    new() { Id = Guid.NewGuid(), Name = "test6" },
                    new() { Id = Guid.NewGuid(), Name = "test7" },
                    new() { Id = Guid.NewGuid(), Name = "test8" },
                    new() { Id = Guid.NewGuid(), Name = "test9" },
                ]);

            base.OnModelCreating(modelBuilder);
        }
    }
}
