using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace LylacParkServices.Interfaces
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
