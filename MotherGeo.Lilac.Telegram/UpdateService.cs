using CefSharp.OffScreen;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using Microsoft.Extensions.Options;
using MotherGeo.Lilac.Telegram.Interfaces;
using MotherGeo.Lilac.Telegram.Model;
using Telegram.Bot.Types.InputFiles;

namespace MotherGeo.Lilac.Telegram
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly BotConfiguration _settings;
        private readonly CameraUrlsConfiguration _cameraConfig;
        private readonly PathForFotosConfiguration _pathConfig;

        private static bool cefInitialized;

        public UpdateService(IBotService botService, 
            IOptions<BotConfiguration> settings, 
            IOptions<CameraUrlsConfiguration> cameraConfig,
             IOptions<PathForFotosConfiguration> path)
        {
            _botService = botService;
            _settings = settings.Value;
            _cameraConfig = cameraConfig.Value;
            _pathConfig = path.Value;

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

        public async Task EchoAsync(RequestUpdate update, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(update.Message.Text) || !(update.Message.Text.Contains("camera1") || update.Message.Text.Contains("camera7")))
            {
                return;
            }

            var url = update.Message.Text.Contains("camera1") ? _cameraConfig.Camera1Url : _cameraConfig.Camera7Url;

            var browser = new ChromiumWebBrowser(url, new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                Javascript = CefState.Enabled,
                ApplicationCache = CefState.Disabled
            });

            while (browser.IsLoading)
            {
                await Task.Delay(500, cancellationToken);
            }

            while (!browser.CanExecuteJavascriptInMainFrame)
            {
                await Task.Delay(500, cancellationToken);
            }

            await Task.Delay(1500, cancellationToken);

            browser.ExecuteScriptAsync("$('.b-embed-logo').remove()");
            await Task.Delay(100, cancellationToken);
            browser.ExecuteScriptAsync("$('.b-embed-btn-play-wrapper').remove()");
            await Task.Delay(100, cancellationToken);

            var screenshot = await browser.ScreenshotAsync();
            if (null == screenshot)
            {
                return;
            }
            var memoryStream = new MemoryStream();
            screenshot.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var photo = new InputOnlineFile(memoryStream);
            await _botService.Client.SendPhotoAsync(update.Message.Chat.Id, photo, cancellationToken: cancellationToken, caption: $"{DateTime.Now:F}");

        }

        public async Task SaveFoto(int number)
        {
            var url = number == 1 ? _cameraConfig.Camera1Url : _cameraConfig.Camera7Url;

            var browser = new ChromiumWebBrowser(url, new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                Javascript = CefState.Enabled,
                ApplicationCache = CefState.Disabled
            });

            while (browser.IsLoading)
            {
                await Task.Delay(500);
            }

            while (!browser.CanExecuteJavascriptInMainFrame)
            {
                await Task.Delay(500);
            }

            browser.ExecuteScriptAsync("$('.b-embed-logo').remove()");
            await Task.Delay(100);
            browser.ExecuteScriptAsync("$('.b-embed-btn-play-wrapper').remove()");
            await Task.Delay(100);

            var screenshot = await browser.ScreenshotAsync();
            if (null == screenshot)
            {
                return;
            }

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream($@"{_pathConfig.Path}{number}\\{DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss")}.png", FileMode.Create, FileAccess.ReadWrite))
                {
                    screenshot.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }


        }
    }
}
