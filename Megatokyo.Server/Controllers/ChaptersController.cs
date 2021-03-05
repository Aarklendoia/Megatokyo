using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Megatokyo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public ChaptersController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet]
        public async Task<List<Chapter>> GetChapters()
        {
            IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindAllAsync();

            List<Chapter> chaptersToSend = new List<Chapter>();

            foreach (Chapters chapter in chapters)
            {
                chaptersToSend.Add(new Chapter
                {
                    Category = chapter.Category,
                    ChapterId = chapter.ChapterId,
                    Number = chapter.Number,
                    Title = chapter.Title
                });
            }

            return chaptersToSend;
        }

        [HttpGet("{category}/{full?}")]
        public async Task<ActionResult<Chapters>> GetByCategory([FromQuery]string category, [FromQuery]bool full = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindByConditionAsync(c => c.Category == category);
            Chapters chapter = chapters.First();
            Chapter chapterToSend;
            if (full)
            {
                chapterToSend = new DetailedChapter
                {
                    Category = chapter.Category,
                    ChapterId = chapter.ChapterId,
                    Number = chapter.Number,
                    Title = chapter.Title
                };
                IEnumerable<Strips> strips = await _repoWrapper.Strips.FindByConditionAsync(s => s.ChapterId == chapter.ChapterId);
                ((DetailedChapter)chapterToSend).LoadStrips(strips);
            }
            else
            {
                chapterToSend = new Chapter
                {
                    Category = chapter.Category,
                    ChapterId = chapter.ChapterId,
                    Number = chapter.Number,
                    Title = chapter.Title
                };
            }

            if (chapter == null)
            {
                return NotFound();
            }

            return Ok(chapterToSend);
        }
    }
}