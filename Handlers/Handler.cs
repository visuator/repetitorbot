using Telegram.Bot.Types;
using User = repetitorbot.Entities.User;

namespace repetitorbot.Handlers;

internal record Context(Update Update, User User);
internal abstract class Handler
{
    public Handler? Next { get; set; }
    public virtual Task Handle(Context context) => Next?.Handle(context) ?? Task.CompletedTask;
}
