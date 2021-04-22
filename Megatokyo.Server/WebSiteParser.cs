using MediatR;
using Megatokyo.Domain;
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
    public class WebSiteParser : IHostedService
    {
        private string _azureConnectionString;
        private string _megatokyoNotificationHub;
        private string _megatokyoArchiveUrl;
        private bool _workInProgress;
        private readonly NotificationHubClient _hub;
        private readonly StripsManager _stripManager;
        private readonly RantsManager _rantManager;
        private readonly FeedManager _feedManager;

        private Timer _timer;

        public WebSiteParser(IMediator mediator)
        {
            LoadConfiguration();
            _hub = NotificationHubClient.CreateClientFromConnectionString(_azureConnectionString, _megatokyoNotificationHub);
            _stripManager = new StripsManager(new Uri(_megatokyoArchiveUrl), mediator);
            _rantManager = new RantsManager(mediator);
            _feedManager = new FeedManager(mediator);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            if (_workInProgress) return;

            bool haveStrips = await _stripManager.CheckIfDataExistsAsync();
            if (!haveStrips)
            {
                _workInProgress = true;
                IList<ChapterDomain> chapters = await _stripManager.ParseChaptersAsync();
                _workInProgress = !await _stripManager.ParseStripsAsync(chapters);
            }

            if (_workInProgress) return;

            bool haveRants = await _rantManager.CheckIfDataExistsAsync();
            if (!haveRants)
            {
                _workInProgress = true;
                _workInProgress = !await _rantManager.ParseRantsAsync();
            }

            await _feedManager.LoadAsync();
            if (_feedManager.Strips.Count > 0)
            {
                IList<ChapterDomain> chapters = await _stripManager.ParseChaptersAsync();
                await _stripManager.ParseStripsAsync(chapters);
            }
            foreach (StripDomain strip in _feedManager.Strips)
            {
                await SendLocalisedStripNotificationsAsync(strip);
            }

            if (_feedManager.Rants.Count > 0)
            {
                await _rantManager.ParseRantsAsync(_feedManager.LastRantNumber);
            }
            foreach (RantDomain rant in _feedManager.Rants)
            {
                await SendLocalisedRantNotifications(rant);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
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
            await _hub.SendTemplateNotificationAsync(data);
        }

        private async Task SendLocalisedRantNotifications(RantDomain rant)
        {
            RantDomain rantToNotify = await _rantManager.GetRantByNumber(rant.Number);
            //NewRantToast newrantToast = new NewRantToast(rantToNotify.Title, rantToNotify.Url, rantToNotify.Author, locale);
            //await SendTemplateNotificationAsync(newrantToast.Toast, locale);
        }

        private async Task SendLocalisedStripNotificationsAsync(StripDomain strip)
        {
            StripDomain stripToNotify = await _stripManager.GetStripByNumberAsync(strip.Number);
            Dictionary<string, string> templateParams = new()
            {
                ["title"] = stripToNotify.Title,
                ["uri"] = stripToNotify.Url.OriginalString,
                ["chapter"] = stripToNotify.Chapter.Number.ToString(CultureInfo.InvariantCulture) + " - " + stripToNotify.Chapter.Title
            };
            await SendTemplateNotificationAsync(templateParams);
        }
    }
}