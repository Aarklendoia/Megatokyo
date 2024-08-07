﻿using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Models;
using Megatokyo.Server.Models.Syndication;
using System.Globalization;

namespace Megatokyo.Server.Models
{
    internal class FeedManager(IMediator mediator)
    {
        public List<Strip> Strips { get; } = [];
        public List<Rant> Rants { get; } = [];
        public int LastStripNumber { get; private set; }
        public int LastRantNumber { get; private set; }

        public async Task LoadAsync()
        {
            Strips.Clear();
            Rants.Clear();

            FeedParser feedParser = new();
            IList<Item> items = feedParser.ParseRss(new Uri("https://megatokyo.com/rss/megatokyo.xml"));

            Checking checking = await mediator.Send(new GetCheckingQuery(1));
            if (checking == null)
            {
                checking = new();
                await mediator.Send(new CreateCheckingCommand(checking));
            }

#if DEBUG
            DateTimeOffset lastCheck = DateTime.Now.AddDays(-700);
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
                        Strip strip = new()
                        {
                            Number = int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        if (await mediator.Send(new GetStripQuery(strip.Number)) != null)
                            Strips.Add(strip);
                        checking.LastStripNumber = strip.Number;
                    }
                    if (item.Title.StartsWith("Rant", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new(item.Title);
                        string number = stringExtractor.Extract("[", "]", false);
                        Rant rant = new()
                        {
                            Number = int.Parse(number, NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        if (await mediator.Send(new GetRantQuery(rant.Number)) != null)
                            Rants.Add(rant);
                        checking.LastRantNumber = rant.Number;
                    }
                }
            }
            checking.LastCheck = DateTimeOffset.UtcNow;
            await mediator.Send(new UpdateCheckingCommand(checking));
        }
    }
}
