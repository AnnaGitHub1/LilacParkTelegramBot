using CefSharp.OffScreen;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.Extensions.Options;
using MotherGeo.Lilac.Telegram.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Update = TelegramBot.Update;

namespace MotherGeo.Lilac.Telegram
{
    public class UpdateService : IUpdateService
    {
        const string testUrl = "https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=458752&width=1280&height=720&lang=ru&fs=&noibw=";

        private readonly IBotService _botService;
        private readonly BotConfiguration _settings;

        private static bool cefInitialized;
        
        public UpdateService(IBotService botService, IOptions<BotConfiguration> settings)
        {
            _botService = botService;
            _settings = settings.Value;
            
            var cefSettings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(_settings.DataPath, "CefSharp\\Cache"),
                IgnoreCertificateErrors = true,
                
            };
            
            if (cefInitialized)
            {
                return;
            }
            
            
            Cef.Initialize(cefSettings, performDependencyCheck: true, browserProcessHandler: null);
            Cef.AddCrossOriginWhitelistEntry("*", "http", "*", true);
            cefInitialized = true;
        }

        private static DateTime _lastRequest = new DateTime(); 

        public async Task EchoAsync(Update update, CancellationToken cancellationToken)
        {
            var browser = new ChromiumWebBrowser(testUrl, new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                Javascript = CefState.Enabled,
                ApplicationCache = CefState.Disabled
            });
            
            while (browser.IsLoading)
            {
                await Task.Delay(100, cancellationToken);
            }

            while (!browser.CanExecuteJavascriptInMainFrame)
            {
                await Task.Delay(100, cancellationToken);
            }

            if (DateTime.Now - _lastRequest > TimeSpan.FromSeconds(60))
            {
                await Task.Delay(5000, cancellationToken);
                
            }
            await Task.Delay(500, cancellationToken);
            _lastRequest = DateTime.Now;

            browser.ExecuteScriptAsync("$('.b-embed-logo').remove()");
            await Task.Delay(100, cancellationToken);
            browser.ExecuteScriptAsync("$('.b-embed-btn-play-wrapper').remove()");
            await Task.Delay(100, cancellationToken);

            var screenshot = await browser.ScreenshotAsync();
            var memoryStream = new MemoryStream();
            screenshot.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin); 
            var photo = new InputOnlineFile(memoryStream);
            await _botService.Client.SendPhotoAsync(update.Message.Chat.ID, photo, cancellationToken: cancellationToken, caption: $"{DateTime.Now:F}");
            
        }
    }
}
