using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TelegramBot;

namespace LylacParkServices.Interfaces
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
