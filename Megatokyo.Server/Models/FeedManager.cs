using Megatokyo.Models;
using Megatokyo.Server.Database;
using Megatokyo.Server.Database.Models;
using Megatokyo.Server.Database.Repository;
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

        public FeedManager(MegatokyoDbContext megatokyoDbContext)
        {
            _repositoryContext = megatokyoDbContext;
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

            FeedParser feedParser = new();
            IList<Item> items = feedParser.ParseRss(new Uri("https://megatokyo.com/rss/megatokyo.xml"));

            IEnumerable<Checking> checkings = await _repository.Checking.FindByConditionAsync(c => c.ChekingId == 1);
            Checking checking;
            if (!checkings.Any())
            {
                checking = new Checking
                {
                    LastCheck = DateTime.MinValue,
                    LastRantNumber = 0,
                    LastStripNumber = 0
                };
                _repository.Checking.Create(checking);
                await _repository.Checking.SaveAsync();
            }
            else
            {
                checking = checkings.First();
            }
            
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
                        StringExtractor stringExtractor = new(item.Title);
                        Strip strip = new()
                        {
                            Number = int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        Strips.Add(strip);
                        checking.LastStripNumber = strip.Number;
                    }
                    if (item.Title.StartsWith("Rant", StringComparison.InvariantCulture))
                    {
                        StringExtractor stringExtractor = new(item.Title);
                        Rant rant = new()
                        {
                            Number = int.Parse(stringExtractor.Extract("[", "]", false), NumberStyles.Integer, CultureInfo.InvariantCulture)
                        };
                        Rants.Add(rant);
                        checking.LastRantNumber = rant.Number;
                    }
                }
            }
            _repository.Checking.Update(checking);
            await _repository.Checking.SaveAsync();
        }
    }
}
