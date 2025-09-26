
using Microsoft.EntityFrameworkCore;
using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

internal class SelectQuizForPublishingHandler(AppDbContext dbContext) : IMiddleware
{
    private const int ItemsPerPage = 6;
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        var count = await dbContext.Quizes.CountAsync(x => !x.Published && x.UserId == context.User.Id);
        var pages = (int)Math.Ceiling(count / (double)ItemsPerPage);

        context.State = new PublishSelectQuizState()
        {
            CurrentPage = 1,
            ItemsPerPage = ItemsPerPage,
            PagesCount = pages,
            WherePublished = false,
            OnlyFromUser = true,
            UserId = context.User.Id
        };

        await next(context);
    }
}
