using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Queries;
using Microsoft.Azure.NotificationHubs;
using System.Globalization;

namespace Megatokyo.Server.Models
{
    internal class WebSiteParser
    {
        private string? _azureConnectionString = string.Empty;
        private string? _megatokyoNotificationHub = string.Empty;
        private string? _megatokyoArchiveUrl = string.Empty;
        private bool _workInProgress;
        private readonly NotificationHubClient _hub;
        private readonly StripsManager _stripManager;
        private readonly RantsManager _rantManager;
        private readonly FeedManager _feedManager;
        private readonly IMediator _mediator;

        public WebSiteParser(IMediator mediator)
        {
            LoadConfiguration();
            _hub = NotificationHubClient.CreateClientFromConnectionString(_azureConnectionString, _megatokyoNotificationHub);
            _stripManager = new StripsManager(new Uri(_megatokyoArchiveUrl), mediator);
            _rantManager = new RantsManager(mediator);
            _feedManager = new FeedManager(mediator);
            _mediator = mediator;
        }

        public async Task ParseAsync()
        {
            if (_workInProgress) return;

            bool haveStrips = await _stripManager.CheckIfDataExistsAsync();
            if (!haveStrips)
            {
                _workInProgress = true;
                IEnumerable<Chapter> chapters = await _stripManager.ParseChaptersAsync();
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
                IEnumerable<Chapter> chapters = await _stripManager.ParseChaptersAsync();
                await _stripManager.ParseStripsAsync(chapters);
            }
            foreach (Strip strip in _feedManager.Strips)
            {
                await SendLocalisedStripNotificationsAsync(strip);
            }

            if (_feedManager.Rants.Count > 0)
            {
                await _rantManager.ParseRantsAsync(_feedManager.LastRantNumber);
            }
            foreach (Rant rant in _feedManager.Rants)
            {
                await SendLocalisedRantNotifications(rant);
            }
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

        private async Task SendLocalisedRantNotifications(Rant rant)
        {
            Rant rantToNotify = await _rantManager.GetRantByNumber(rant.Number);
            //NewRantToast newrantToast = new NewRantToast(rantToNotify.Title, rantToNotify.Url, rantToNotify.Author, locale);
            //await SendTemplateNotificationAsync(newrantToast.Toast, locale);
        }

        private async Task SendLocalisedStripNotificationsAsync(Strip strip)
        {
            Strip stripToNotify = await _stripManager.GetStripByNumberAsync(strip.Number);
            Chapter chapter = await _mediator.Send(new GetChapterQuery(stripToNotify.Category));
            Dictionary<string, string> templateParams = new()
            {
                ["title"] = stripToNotify.Title,
                ["uri"] = (stripToNotify.Url != null) ? stripToNotify.Url.OriginalString : "",
                ["chapter"] = chapter.Number.ToString(CultureInfo.InvariantCulture) + " - " + chapter.Title
            };
            await SendTemplateNotificationAsync(templateParams);
        }
    }
}
