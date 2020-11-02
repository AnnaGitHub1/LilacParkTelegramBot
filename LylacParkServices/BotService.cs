using LylacParkServices.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace LylacParkServices
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;

            Client = new TelegramBotClient(_config.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
