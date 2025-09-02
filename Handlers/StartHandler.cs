using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;
namespace repetitorbot.Handlers;

internal class StartHandler(AppDbContext dbContext) : Handler
{
    private const int ItemsPerPage = 6;
    public override async Task Handle(Context context)
    {
        var user = context.User;
        var count = await dbContext.Quizes.CountAsync();
        var pages = count / ItemsPerPage;

        SelectQuizState state = new()
        {
            CurrentPage = 1,
            PagesCount = pages,
            ItemsPerPage = ItemsPerPage
        };

        user.State = state;
        await dbContext.SaveChangesAsync();

        await base.Handle(context);
    }
}
