using Microsoft.EntityFrameworkCore;
using repetitorbot.Handlers;

namespace repetitorbot.Middlewares;

internal class StateMiddleware(AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        context.State = await dbContext.States.SingleOrDefaultAsync(x => x.UserId == context.User.Id);
        await next(context);
    }
}
