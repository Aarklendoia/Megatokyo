using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Models;
using Megatokyo.Server.Models.Syndication;
using System.Globalization;

namespace Megatokyo.Server.Models
{
    internal class FeedManager
    {
        private readonly IMediator _mediator;

        public List<Strip> Strips { get; }
        public List<Rant> Rants { get; }
        public int LastStripNumber { get; private set; }
        public int LastRantNumber { get; private set; }

        public FeedManager(IMediator mediator)
        {
            _mediator = mediator;
            Strips = new List<Strip>();
            Rants = new List<Rant>();
        }

        public async Task LoadAsync()
        {
            Strips.Clear();
            Rants.Clear();

            FeedParser feedParser = new();
            IList<Item> items = feedParser.ParseRss(new Uri("https://megatokyo.com/rss/megatokyo.xml"));

            Checking checking = await _mediator.Send(new GetCheckingQuery(1));
            if (checking == null)
            {
                checking = new(DateTimeOffset.MinValue, 0, 0);
                await _mediator.Send(new CreateCheckingCommand(checking));
            }

#if DEBUG
            DateTimeOffset lastCheck = DateTime.Now.AddDays(-30);
#else
            DateTimeOffset lastCheck = checking.LastCheck;
#endif
            LastStripNumber = checking.LastStripNumber;
            LastRantNumber = checking.LastRantNumber;

            foreach (Item item in items)
            {
                if (item.PublishDate > lastCheck)
                {
                    if (item.Title.StartsWith("Comic", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new(item.Title);
                        Strip strip = new(int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture));
                        Strips.Add(strip);
                        checking.LastStripNumber = strip.Number;
                    }
                    if (item.Title.StartsWith("Rant", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new(item.Title);
                        Rant rant = new(int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture));
                        Rants.Add(rant);
                        checking.LastRantNumber = rant.Number;
                    }
                }
            }
            await _mediator.Send(new UpdateCheckingCommand(checking));
        }
    }
}
