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

        public async Task EchoAsync(Update update)
        {
            
            var browser = new ChromiumWebBrowser(testUrl, new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                Javascript = CefState.Enabled,
                
            });
            
            while (browser.IsLoading)
            {
                await Task.Delay(100);
            }
            
            //browser.Load("https://afi-park.ru/");
            await Task.Delay(7000);
            browser.ExecuteScriptAsync("$('div.b-embed-btn-play').click()");
            await Task.Delay(3000);
            await saveScreenShot(browser);
            
            browser.ExecuteScriptAsync("$('button.iv-btn').click()");
            await Task.Delay(3000);
            await saveScreenShot(browser);
            
            browser.ExecuteScriptAsync("$('button.iv-ui-btn__m-theme-contour').click()");
            await Task.Delay(3000);
            await saveScreenShot(browser);
            
            browser.ExecuteScriptAsync("$('div.b-embed-btn-play').click()");
            await Task.Delay(3000);
            await saveScreenShot(browser);
            
           /* browser.ExecuteScriptAsync("$(\"[data-targ='webcam']\").click()");
            await Task.Delay(5000);
            
            await Task.Delay(5000);
            browser.ExecuteScriptAsync("$('button.iv-camera-video-view-fullscreen-button').click()");
            await Task.Delay(7000);*/
            //browser.Reload(true);
            //await Task.Delay(7000);
            
            var s = await browser.GetSourceAsync();
            
           /* await using (var stream = File.Open(screenshotPath, FileMode.Open))
            {
                var photo = new InputOnlineFile(stream);
                var send = await _botService.Client.SendPhotoAsync(update.Message.Chat.ID, photo, $"Фото ЖК на {DateTime.Now:yyyy-MM-dd HH:mm}");
            }

             DeleteScreenshot(screenshotPath);*/

        }
        
        private static int _k = 0;

        private async Task saveScreenShot(ChromiumWebBrowser browser)
        {
            var screenshot = await browser.ScreenshotAsync();

            var guid = Guid.NewGuid();

            var screenshotPath = Path.Combine(_settings.DataPath, $"{_k++}-screenshot_{guid}.png");

            screenshot.Save(screenshotPath);

            screenshot.Dispose();
            //var s = await browser.GetSourceAsync();
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
