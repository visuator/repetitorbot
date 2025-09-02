using Telegram.Bot.Types;

namespace repetitorbot
{
    internal static class TelegramExtensions
    {
        public static long GetUserId(this Message message) => message.Chat.Id;
        public static long GetUserId(this CallbackQuery query) => query.From.Id;
        public static long GetChatId(this Update update) => update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id ?? throw new InvalidOperationException("chat was not found");
    }
}
