using Microsoft.Extensions.DependencyInjection.Extensions;
using repetitorbot.Handlers;

namespace repetitorbot.Services;

internal class PipelineBuilder(IServiceCollection services)
{
    private readonly List<Func<IServiceProvider, UpdateDelegate, UpdateDelegate>> _components = [];

    public PipelineBuilder UseMiddleware<TMiddleware>() where TMiddleware : class, IMiddleware
    {
        services.TryAddScoped<TMiddleware>();

        _components.Add(CreateMiddleware<TMiddleware>);
        return this;
    }

    public void Build()
    {
        Func<IServiceProvider, UpdateDelegate> pipeline = _ => _ => Task.CompletedTask;
        for (var i = _components.Count - 1; i >= 0; i--)
        {
            pipeline = provider => _components[i](provider, pipeline(provider));
        }
        services.AddScoped(provider => pipeline(provider));
    }

    private static UpdateDelegate CreateMiddleware<TMiddleware>(IServiceProvider provider, UpdateDelegate next) where TMiddleware : IMiddleware
    {
        return async context =>
        {
            var instance = provider.GetRequiredService<TMiddleware>();
            await instance.Invoke(context, next);
        };
    }
}
