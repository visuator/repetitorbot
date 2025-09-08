namespace repetitorbot.Handlers;

internal class SkipQuestionMiddleware : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        await next(context);
    }
}
