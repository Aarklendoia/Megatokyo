using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Models.Parsers;

namespace Megatokyo.Server.Models
{
    internal class StripsManager(Uri url, IMediator mediator)
    {

        public Uri Url { get; set; } = url;

        public async Task<IEnumerable<Chapter>> ParseChaptersAsync()
        {
            IEnumerable<Chapter> chapters = ChaptersParser.Parse(Url);
            IEnumerable<Chapter> chaptersInDatabase = await mediator.Send(new GetAllChaptersQuery());
            foreach (Chapter chapter in chapters)
            {
                if (!chaptersInDatabase.Any(c => c.Category == chapter.Category))
                {
                    Chapter newChapter = new()
                    {
                        Number = chapter.Number,
                        Category = chapter.Category,
                        Title = chapter.Title
                    };
                    await mediator.Send(new CreateChapterCommand(newChapter));
                }
            }
            await mediator.Send(new SaveChapterCommand());
            return chapters;
        }

        public async Task<bool> ParseStripsAsync(IEnumerable<Chapter> chapters)
        {
            IEnumerable<Strip> stripsInDatabase = await mediator.Send(new GetAllStripsQuery());
            List<Strip> strips = await StripsParser.ParseAsync(Url, chapters, stripsInDatabase);
            foreach (Strip strip in strips)
            {
                if (!stripsInDatabase.Any(s => s.Number == strip.Number))
                {
                    Chapter currentChapter = await mediator.Send(new GetChapterQuery(strip.Category));
                    Strip newStrip = new()
                    {
                        Number = strip.Number,
                        Title = strip.Title,
                        PublishDate = strip.PublishDate,
                        Category = currentChapter.Category,
                        Url = strip.Url
                    };
                    await mediator.Send(new CreateStripCommand(newStrip));
                }
            }
            await mediator.Send(new SaveStripCommand());
            return true;
        }

        public async Task<Strip> GetStripByNumberAsync(int number)
        {
            return await mediator.Send(new GetStripQuery(number));
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Strip> strips = await mediator.Send(new GetAllStripsQuery());
            return strips.Any();
        }
    }
}
