using Telegram.Bot;

namespace MotherGeo.Lilac.Telegram.Interfaces
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
