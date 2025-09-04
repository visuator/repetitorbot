using System;
using repetitorbot.Handlers;

namespace repetitorbot.Services.Common;

internal interface IHandler
{
    Task Handle(Context context);
}
