using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MotherGeo.Lilac.Telegram.Interfaces
{
    public interface ISaveService
    {
        Task SaveFoto(int number);
    }
}
