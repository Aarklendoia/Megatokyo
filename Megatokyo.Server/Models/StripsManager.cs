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

        /// <summary>
        /// Extrait du site de Megatokyo les chapitres et les planches puis les stocke en base de données.
        /// </summary>
        /// <param name="url">URL de la page d'archives de Megatokyo.</param>
        /// <param name="mediator"></param>
        public StripsManager(Uri url, IMediator mediator)
        {
            Url = url;
            _mediator = mediator;
        }

        /// <summary>
        /// Extrait les chapitres puis les stocke en base de données.
        /// </summary>
        public async Task<ChaptersDomain> ParseChaptersAsync()
        {
            ChaptersDomain chapters = ChaptersParser.Parse(Url);

            IEnumerable<ChapterDomain> chaptersInDatabase = await _mediator.Send(new GetAllChaptersQuery());

            foreach (ChapterDomain chapter in chapters)
            {
                if (!chaptersInDatabase.Where(c => c.Number == chapter.Number).Any())
                {
                    ChapterDomain newChapter = new(chapter.Number, chapter.Title, chapter.Category);
                    await _mediator.Send(new CreateChapterCommand(newChapter));
                }
            }
            await _mediator.Send(new SaveChapterCommand());
            return chapters;
        }

        /// <summary>
        /// Recherches toutes les planches des chapitres fournis.
        /// </summary>
        /// <param name="chapters">Liste des chapitres pour lesquels il faut rechercher les planches.</param>
        /// <returns></returns>
        public async Task<bool> ParseStripsAsync(ChaptersDomain chapters)
        {
            IEnumerable<StripDomain> stripsInDatabase = await _mediator.Send(new GetAllStripsQuery());

            List<StripDomain> strips = await StripsParser.ParseAsync(Url, chapters, stripsInDatabase);

            IEnumerable<ChapterDomain> chaptersInDatabase = await _mediator.Send(new GetAllChaptersQuery());
            foreach (StripDomain strip in strips)
            {
                if (!stripsInDatabase.Where(s => s.Number == strip.Number).Any())
                {
                    ChapterDomain currentChapter = chaptersInDatabase.Where(c => c.Category == strip.Category).First();
                    StripDomain newStrip = new(currentChapter, strip.Number, strip.Title, strip.Url, strip.Timestamp);
                    await _mediator.Send(new CreateStripCommand(newStrip));
                }
            }
            await _mediator.Send(new SaveStripCommand());
            return true;
        }

        public async Task<StripDomain> GetStripByNumberAsync(int number)
        {
            return await _mediator.Send(new GetStripQuery(number));
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<StripDomain> strips = await _mediator.Send(new GetAllStripsQuery());
            return strips.Any();
        }
    }
}
