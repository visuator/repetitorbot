using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

internal class BackPageHandler : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not SelectQuizState state)
        {
            return;
        }

        if (state.CurrentPage - 1 < 1)
        {
            return;
        }

        state.CurrentPage--;

        await next(context);
    }
}
internal class ForwardPageHandler : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        if (context.State is not SelectQuizState state)
        {
            return;
        }

        if (state.CurrentPage + 1 > state.PagesCount)
        {
            return;
        }

        state.CurrentPage++;

        await next(context);
    }
}
