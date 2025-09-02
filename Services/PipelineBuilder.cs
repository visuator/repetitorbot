using repetitorbot.Handlers;

namespace repetitorbot.Services;

internal class PipelineBuilder(IServiceProvider provider)
{
    private Handler _head = null!;
    private Handler _current = null!;

    public Handler Result => _head;

    public PipelineBuilder Use<THandler>() where THandler : Handler
    {
        var handler = provider.GetRequiredService<THandler>();

        if (_head is null)
        {
            _head = handler;
            _current = handler;
        }
        else
        {
            _current.Next = handler;
        }
        return this;
    }
}
