namespace repetitorbot.Handlers;

internal class ViewListHandler : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        await next(context);
    }
}
