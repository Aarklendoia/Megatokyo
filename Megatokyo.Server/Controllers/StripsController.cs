using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public StripsController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        private async Task<List<Strip>> LoadData()
        {
            List<Strip> stripsData = new();
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
            IEnumerable<bool> stripSelected = stripsData.Select(s => s.Number == number);
            if (!stripSelected.Any())
            {
                return NotFound();
            }
            Strip strip = stripsData.First();
            if (full)
            {
                IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindByConditionAsync(c => c.ChapterId == strip.ChapterId);
                Chapters chapter = chapters.First();
                DetailedStrip detailedStrip = new(strip);
                detailedStrip.LoadChapter(chapter);
                return Ok(detailedStrip);
            }
            return Ok(strip);
        }
    }
}