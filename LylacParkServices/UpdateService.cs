using CefSharp.OffScreen;
using LylacParkServices.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using TelegramBot;

namespace LylacParkServices
{
    public class UpdateService : IUpdateService
    {
        const string testUrl = "https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=458752&width=1280&height=720&lang=ru&ap=&fs=&noibw=";

        private static ChromiumWebBrowser browser;

        private readonly IBotService _botService;

        public UpdateService(IBotService botService)
        {
            _botService = botService;
        }

        public async Task EchoAsync(Update update)
        {
            browser = new ChromiumWebBrowser(testUrl);

            Thread.Sleep(1000);

            var screenshot = await browser.ScreenshotAsync();

            var guid = Guid.NewGuid();

            var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"screenshot_{guid}.png");

            screenshot.Save(screenshotPath);

            screenshot.Dispose();

            Process.Start(new ProcessStartInfo(screenshotPath)
            {
                // UseShellExecute is false by default on .NET Core.
                UseShellExecute = false
            });

            using (var stream = File.Open(screenshotPath, FileMode.Open))
            {
                InputOnlineFile foto = new InputOnlineFile(stream);

                var send = await _botService.Client.SendPhotoAsync(update.Message.Chat.ID, foto, $"Фото ЖК на {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
            }

            DeleteScreenshot(screenshotPath);

        }

        private void DeleteScreenshot(string screenPath)
        {
            FileInfo fileInf = new FileInfo(screenPath);

            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }
    }
}
