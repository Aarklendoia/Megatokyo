using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Megatokyo.Server.Controllers
{
    public static class CacheKeys
    {
        public static string Strips { get { return "_AllStrips"; } }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class StripsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IMemoryCache _cache;

        public StripsController(IRepositoryWrapper repoWrapper, IMemoryCache memoryCache)
        {
            _repoWrapper = repoWrapper;
            _cache = memoryCache;
        }

        private async Task<List<Strip>> LoadData()
        {
            if (!_cache.TryGetValue(CacheKeys.Strips, out List<Strip> stripsData))
            {
                stripsData = new List<Strip>();
                IEnumerable<Strips> strips = await _repoWrapper.Strips.FindAllAsync();
                foreach (Strips strip in strips)
                {
                    stripsData.Add(new Strip
                    {
                        ChapterId = strip.ChapterId,
                        Date = strip.Date,
                        Number = strip.Number,
                        StripId = strip.StripId,
                        Title = strip.Title,
                        Url = strip.Url
                    });
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                _cache.Set(CacheKeys.Strips, stripsData, cacheEntryOptions);
            }
            return stripsData;
        }

        [HttpGet]
        public async Task<List<Strip>> GetStrips()
        {
            return await LoadData();
        }

        [HttpGet("{number}/{full?}")]
        public async Task<ActionResult<Strip>> GetStrips([FromRoute] int number, bool full = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<Strip> stripsData = await LoadData();
            stripsData.Select(s => s.Number == number);
            Strip strip = stripsData.First();
            if (strip == null)
            {
                return NotFound();
            }
            if (full)
            {
                IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindByConditionAsync(c => c.ChapterId == strip.ChapterId);
                Chapters chapter = chapters.First();
                DetailedStrip detailedStrip = new DetailedStrip(strip);
                detailedStrip.LoadChapter(chapter);
                return Ok(detailedStrip);
            }
            return Ok(strip);
        }
    }
}