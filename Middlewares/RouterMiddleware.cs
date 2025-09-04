using Microsoft.Extensions.DependencyInjection.Extensions;
using repetitorbot.Handlers;
using repetitorbot.Services.Common;

namespace repetitorbot.Middlewares;

internal delegate Task<bool> Route(IServiceProvider serviceProvider, Context context);
internal class RouterMiddleware(IServiceProvider provider, IEnumerable<Route> routes) : IMiddleware
{
    public async Task Invoke(Context context, UpdateDelegate next)
    {
        foreach (var route in routes)
        {
            if (await route(provider, context))
                return;
        }
        await next(context);
    }
}
internal class RouteBuilder(IServiceCollection services)
{
    private List<Route> _routes { get; } = [];

    public RouteBuilder Command<THandler>(string name) where THandler : class, IHandler
    {
        services.TryAddScoped<THandler>();
        
        _routes.Add(async (provider, context) =>
        {
            var text = context.Update.GetMessageText();

            if (text.StartsWith('/') && text.AsSpan()[1..].Equals(name))
            {
                await provider.GetRequiredService<THandler>().Handle(context);
                return true;
            }

            return false;
        });
        return this;
    }

    public RouteBuilder RouteWhen<THandler>(Func<Context, bool> predicate) where THandler : class, IHandler
    {
        services.TryAddScoped<THandler>();

        _routes.Add(async (provider, context) =>
        {
            if (predicate(context))
            {
                await provider.GetRequiredService<THandler>().Handle(context);
                return true;
            }
            return false;
        });
        return this;
    }

    public void Build()
    {
        foreach (var route in _routes)
        {
            services.AddSingleton(route);
        }
    }
}