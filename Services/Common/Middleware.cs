using repetitorbot.Entities.States;
using Telegram.Bot.Types;
using User = repetitorbot.Entities.User;

namespace repetitorbot.Handlers;

internal record Context(Update Update)
{
    public User User { get; set; } = null!;
    public State? State { get; set; }
}
internal delegate Task UpdateDelegate(Context context);
internal interface IMiddleware
{
    Task Invoke(Context context, UpdateDelegate next);
}
