using repetitorbot.Handlers;

namespace repetitorbot.Middlewares;

internal class UserMiddleware(AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        var user = dbContext.Users.SingleOrDefault(x => x.Id == context.Update.GetChatId());
        if (user is null)
        {
            user = new();
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }
        context.User = user;
        await next(context);
    }
}
