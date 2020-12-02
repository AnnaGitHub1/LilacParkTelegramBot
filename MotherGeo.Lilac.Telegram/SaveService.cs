using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Extensions.Options;
using MotherGeo.Lilac.Telegram.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MotherGeo.Lilac.Telegram
{
    public class SaveService : ISaveService
    {
        private readonly CameraUrlsConfiguration _config;

        private readonly PathForFotosConfiguration _pathConfig;

        public SaveService(IOptions<CameraUrlsConfiguration> config, IOptions<PathForFotosConfiguration> path)
        {
            _config = config.Value;
            _pathConfig = path.Value;
        }

        public async Task SaveFoto(int number)
        {
            try
            {
                var url = number == 1 ? _config.Camera1Url : _config.Camera7Url;

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

            catch(Exception e)
            {

            }
        }
    }
}
