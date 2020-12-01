using System.Threading;
using System.Threading.Tasks;
using MotherGeo.Lilac.Telegram.Model;

namespace MotherGeo.Lilac.Telegram.Interfaces
{
    public interface IUpdateService
    {
        Task EchoAsync(RequestUpdate update, CancellationToken cancellationToken);
    }
}
