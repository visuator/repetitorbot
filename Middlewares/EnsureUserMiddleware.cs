using repetitorbot.Handlers;

namespace repetitorbot.Middlewares;

internal class EnsureUserMiddleware(AppDbContext dbContext) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        var user = dbContext.Users.SingleOrDefault(x => x.Id == context.Update.GetChatId());
        if (user is null)
        {
            user = new() { Id = context.Update.GetChatId() };
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }
        context.User = user;
        await next(context);
    }
}
