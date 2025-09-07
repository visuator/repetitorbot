using repetitorbot.Entities.States;

namespace repetitorbot.Handlers;

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