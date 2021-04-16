using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        [ProducesResponseType(typeof(List<Chapter>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetChapters))]
        public async Task<IActionResult> GetChapters()
        {
            IEnumerable<Chapters> chapters = await _repoWrapper.Chapters.FindAllAsync();

            List<Chapter> chaptersToSend = new();

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

            return Ok(chaptersToSend);
        }


        [ProducesResponseType(typeof(Chapters), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{category}/{full?}", Name = nameof(GetByCategory))]
        public async Task<IActionResult> GetByCategory([FromRoute] string category, bool full = false)
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