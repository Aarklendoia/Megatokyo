using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Models.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    internal class StripsManager
    {
        private readonly IMediator _mediator;

        public Uri Url { get; set; }

        public StripsManager(Uri url, IMediator mediator)
        {
            Url = url;
            _mediator = mediator;
        }

        public async Task<IEnumerable<Chapter>> ParseChaptersAsync()
        {
            IEnumerable<Chapter> chapters = ChaptersParser.Parse(Url);
            IEnumerable<Chapter> chaptersInDatabase = await _mediator.Send(new GetAllChaptersQuery());
            foreach (Chapter chapter in chapters)
            {
                if (!chaptersInDatabase.Where(c => c.Category == chapter.Category).Any())
                {
                    Chapter newChapter = new(chapter.Number, chapter.Title, chapter.Category);
                    await _mediator.Send(new CreateChapterCommand(newChapter));
                }
            }
            await _mediator.Send(new SaveChapterCommand());
            return chapters;
        }

        public async Task<bool> ParseStripsAsync(IEnumerable<Chapter> chapters)
        {
            IEnumerable<Strip> stripsInDatabase = await _mediator.Send(new GetAllStripsQuery());
            List<Strip> strips = await StripsParser.ParseAsync(Url, chapters, stripsInDatabase);
            foreach (Strip strip in strips)
            {
                if (!stripsInDatabase.Where(s => s.Number == strip.Number).Any())
                {
                    Chapter currentChapter = await _mediator.Send(new GetChapterQuery(strip.Category));
                    Strip newStrip = new(currentChapter.Category, strip.Number, strip.Title, strip.Url, strip.PublishDate);
                    await _mediator.Send(new CreateStripCommand(newStrip));
                }
            }
            await _mediator.Send(new SaveStripCommand());
            return true;
        }

        public async Task<Strip> GetStripByNumberAsync(int number)
        {
            return await _mediator.Send(new GetStripQuery(number));
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Strip> strips = await _mediator.Send(new GetAllStripsQuery());
            return strips.Any();
        }
    }
}
