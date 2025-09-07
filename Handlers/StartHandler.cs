using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

internal class StartHandler(AppDbContext dbContext) : IMiddleware
{
    private const int ItemsPerPage = 6;
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        var count = await dbContext.Quizes.CountAsync();
        var pages = (int)Math.Ceiling(count / (double)ItemsPerPage);

        context.State = new SelectQuizState()
        {
            CurrentPage = 1,
            ItemsPerPage = ItemsPerPage,
            PagesCount = pages,
            UserId = context.User.Id
        };

        await next(context);
    }
}
