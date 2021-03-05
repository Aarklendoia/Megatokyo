using Megatokyo.Models;
using Megatokyo.Server.Database;
using Megatokyo.Server.Database.Models;
using Megatokyo.Server.Database.Repository;
using Megatokyo.Server.Models.Entities;
using Megatokyo.Server.Models.Syndication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    internal class FeedManager: IDisposable
    {
        private readonly MegatokyoDbContext _repositoryContext;
        private readonly RepositoryWrapper _repository;

        public List<Strip> Strips { get; }
        public List<Rant> Rants { get; }
        public int LastStripNumber { get; private set; }
        public int LastRantNumber { get; private set; }

        public FeedManager()
        {
            _repositoryContext = new MegatokyoDbContext();
            _repository = new RepositoryWrapper(_repositoryContext);
            Strips = new List<Strip>();
            Rants = new List<Rant>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repositoryContext.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public async Task LoadAsync()
        {
            Strips.Clear();
            Rants.Clear();

            FeedParser feedParser = new FeedParser();
            IList<Item> items = feedParser.ParseRss(new Uri("https://megatokyo.com/rss/megatokyo.xml"));

            IEnumerable<Checking> checkings = await _repository.Checking.FindByConditionAsync(c => c.ChekingId == 1).ConfigureAwait(false);
            Checking checking = checkings.First();
#if DEBUG
            DateTime lastCheck = DateTime.Now.AddDays(-30);
#else
            DateTime lastCheck = checking.LastCheck;
#endif
            LastStripNumber = checking.LastStripNumber;
            LastRantNumber = checking.LastRantNumber;

            foreach (Item item in items)
            {
                if (item.PublishDate > lastCheck)
                {
                    if (item.Title.StartsWith("Comic", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new StringExtractor(item.Title);
                        Strip strip = new Strip
                        {
                            Number = int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        Strips.Add(strip);
                        checking.LastStripNumber = strip.Number;
                    }
                    if (item.Title.StartsWith("Rant", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new StringExtractor(item.Title);
                        Rant rant = new Rant
                        {
                            Number = int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        Rants.Add(rant);
                        checking.LastRantNumber = rant.Number;
                    }
                }
            }
            _repository.Checking.Update(checking);
            await _repository.Checking.SaveAsync().ConfigureAwait(false);
        }
    }
}
