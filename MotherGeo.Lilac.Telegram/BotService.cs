using Microsoft.Extensions.Options;
using MotherGeo.Lilac.Telegram.Interfaces;
using Telegram.Bot;

namespace MotherGeo.Lilac.Telegram
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
