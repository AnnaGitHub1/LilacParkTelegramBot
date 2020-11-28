using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.Extensions.Options;
using MotherGeo.Lilac.Telegram.Interfaces;
using Telegram.Bot.Types.InputFiles;
using TelegramBot;

namespace MotherGeo.Lilac.Telegram
{
    public class UpdateService : IUpdateService
    {
        const string testUrl = "https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=458752&width=1280&height=720&lang=ru&ap=&fs=&noibw=";

        private readonly IBotService _botService;
        private readonly BotConfiguration _settings;
        
        public UpdateService(IBotService botService, IOptions<BotConfiguration> settings)
        {
            _botService = botService;
            _settings = settings.Value;
            
            var cefSettings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(_settings.DataPath, "CefSharp\\Cache")
            };
            Cef.Initialize(cefSettings, performDependencyCheck: true, browserProcessHandler: null);

            
            
        }

        public async Task EchoAsync(Update update)
        {
            var browser = new ChromiumWebBrowser("https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=458752&width=1280&height=720&lang=ru&ap=&fs=&noibw=");
            
            while (browser.IsLoading)
            {
                await Task.Delay(100);
            }
            
            //browser.Load("https://afi-park.ru/");
            await Task.Delay(7000);
            browser.Reload(true);
            await Task.Delay(7000);
            var screenshot = await browser.ScreenshotAsync();

            var guid = Guid.NewGuid();

            var screenshotPath = Path.Combine(_settings.DataPath, $"screenshot_{guid}.png");

            screenshot.Save(screenshotPath);

            screenshot.Dispose();

            await using (var stream = File.Open(screenshotPath, FileMode.Open))
            {
                var photo = new InputOnlineFile(stream);
                var send = await _botService.Client.SendPhotoAsync(update.Message.Chat.ID, photo, $"Фото ЖК на {DateTime.Now:yyyy-MM-dd HH:mm}");
            }

            DeleteScreenshot(screenshotPath);

        }

        private static void DeleteScreenshot(string screenPath)
        {
            var fileInf = new FileInfo(screenPath);

            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }
    }
}
