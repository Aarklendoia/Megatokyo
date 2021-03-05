using Megatokyo.Server.Models;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Server
{
    public class WebSiteParser : IHostedService, IDisposable
    {
        private string _azureConnectionString;
        private string _megatokyoNotificationHub;
        private string _megatokyoArchiveUrl;
        private readonly NotificationHubClient _hub;
        private readonly StripsManager _stripManager;
        private readonly RantsManager _rantManager;
        private readonly FeedManager _feedManager;

        private Timer _timer;

        public WebSiteParser()
        {
            LoadConfiguration();
            _hub = NotificationHubClient.CreateClientFromConnectionString(_azureConnectionString, _megatokyoNotificationHub);
            _stripManager = new StripsManager(new Uri(_megatokyoArchiveUrl));
            _rantManager = new RantsManager();
            _feedManager = new FeedManager();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            bool haveStrips = await _stripManager.CheckIfDataExistsAsync().ConfigureAwait(false);
            if (!haveStrips)
            {
                IList<Chapter> chapters = await _stripManager.ParseChaptersAsync().ConfigureAwait(false);
                await _stripManager.ParseStripsAsync(chapters).ConfigureAwait(false);
            }
            bool haveRants = await _rantManager.CheckIfDataExistsAsync().ConfigureAwait(false);
            if (!haveRants)
            {
                await _rantManager.ParseRantsAsync().ConfigureAwait(false);
            }

            //await _feedManager.LoadAsync().ConfigureAwait(false);
            //if (_feedManager.Strips.Count > 0)
            //{
            //    IList<Chapter> chapters = await _stripManager.ParseChaptersAsync().ConfigureAwait(false);
            //    await _stripManager.ParseStripsAsync(chapters).ConfigureAwait(false);
            //}
            //foreach (Strip strip in _feedManager.Strips)
            //{
            //    await SendLocalisedStripNotificationsAsync(strip).ConfigureAwait(false);
            //}

            //if (_feedManager.Rants.Count > 0)
            //{
            //    await _rantManager.ParseRantsAsync(_feedManager.LastRantNumber).ConfigureAwait(false);
            //}
            //foreach (Rant rant in _feedManager.Rants)
            //{
            //    await SendLocalisedRantNotifications(rant).ConfigureAwait(false);
            //}
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
            
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
                _stripManager.Dispose();
                _rantManager.Dispose();
                _feedManager.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void LoadConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _azureConnectionString = configuration.GetConnectionString("AzurePushConnection");
            _megatokyoNotificationHub = configuration.GetConnectionString("MegatokyoHub");
            _megatokyoArchiveUrl = configuration.GetConnectionString("MegatokyoArchiveUrl");
        }

        private async Task SendTemplateNotificationAsync(Dictionary<string, string> data)
        {
            await _hub.SendTemplateNotificationAsync(data).ConfigureAwait(false);
        }

        private async Task SendLocalisedRantNotifications(Rant rant)
        {
            Rant rantToNotify = await _rantManager.GetRantByNumber(rant.Number).ConfigureAwait(false);
            //NewRantToast newrantToast = new NewRantToast(rantToNotify.Title, rantToNotify.Url, rantToNotify.Author, locale);
            //await SendTemplateNotificationAsync(newrantToast.Toast, locale);
        }

        private async Task SendLocalisedStripNotificationsAsync(Strip strip)
        {
            DetailedStrip stripToNotify = await _stripManager.GetStripByNumberAsync(strip.Number).ConfigureAwait(false);
            Dictionary<string, string> templateParams = new Dictionary<string, string>
            {
                ["title"] = stripToNotify.Title,
                ["uri"] = stripToNotify.Url.OriginalString,
                ["chapter"] = stripToNotify.Chapter.Number.ToString(CultureInfo.InvariantCulture) + " - " + stripToNotify.Chapter.Title
            };
            await SendTemplateNotificationAsync(templateParams).ConfigureAwait(false);
        }
    }
}