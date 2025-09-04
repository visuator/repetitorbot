﻿using repetitorbot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace repetitorbot
{
    internal class UpdateHandler(UpdateDelegate pipeline) : IUpdateHandler
    {
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) => pipeline(new Context(update));
    }
}
