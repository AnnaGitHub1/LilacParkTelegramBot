﻿using CefSharp.OffScreen;
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
        private const string camera1Url = "https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=458752&width=1280&height=720&lang=ru&fs=&noibw=";
        private const string camera7Url =
            "https://open.ivideon.com/embed/v2/?server=100-gHmJOwS7jHDJLZm8nXlT3w&camera=524288&width=1920&height=1080&lang=ru&fs=&noibw=";
        
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

        public async Task EchoAsync(RequestUpdate update, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(update.Message.Text) || !(update.Message.Text.Contains("camera1") || update.Message.Text.Contains("camera7")))
            {
                return;
            }

            var url = update.Message.Text.Contains("camera1") ? camera1Url : camera7Url;
            
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
    }
}
