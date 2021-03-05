using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    public class Home
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public DetailedStrip Strip { get; private set; }
        public Rants Rant { get; private set; }

        private async Task<DetailedStrip> GetStrip()
        {
            Strips strips = await _repoWrapper.Strips.LatestAsync(s => s.Date).ConfigureAwait(false);
            DetailedStrip strip = new DetailedStrip
            {
                ChapterId = strips.ChapterId,
                Date = strips.Date,
                Number = strips.Number,
                StripId = strips.StripId,
                Title = strips.Title,
                Url = strips.Url
            };
            IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindByConditionAsync(c => c.ChapterId == strips.ChapterId).ConfigureAwait(false);
            Chapters chapter = chapters.First();
            strip.LoadChapter(chapter);
            return strip;
        }

        private async Task<Rants> GetRant()
        {
            return await _repoWrapper.Rants.LatestAsync(r => r.Date).ConfigureAwait(false);
        }

        public Home(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        internal async Task Load()
        {
            Strip = await GetStrip().ConfigureAwait(false);
            Rant = await GetRant().ConfigureAwait(false);
        }
    }
}