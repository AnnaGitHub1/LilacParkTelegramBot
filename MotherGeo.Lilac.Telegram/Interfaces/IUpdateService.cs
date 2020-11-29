using System.Threading;
using System.Threading.Tasks;
using TelegramBot;

namespace MotherGeo.Lilac.Telegram.Interfaces
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update, CancellationToken cancellationToken);
    }
}
