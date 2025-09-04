using repetitorbot.Middlewares;
using repetitorbot.Services;

namespace repetitorbot.Extensions;

internal static class PipelineExtensions
{
    public static IServiceCollection AddPipeline(this IServiceCollection services, Action<PipelineBuilder> configure)
    {
        PipelineBuilder instance = new(services);

        configure(instance);

        instance.Build();

        return services;
    }
    
    public static IServiceCollection AddRouting(this IServiceCollection services, Action<RouteBuilder> configure)
    {
        RouteBuilder instance = new(services);

        configure(instance);

        instance.Build();

        return services;
    }
}
