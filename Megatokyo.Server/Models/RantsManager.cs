using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Models.Parsers;

namespace Megatokyo.Server.Models
{
    internal class RantsManager(IMediator mediator)
    {
        public async Task<bool> ParseRantsAsync()
        {
            return await ParseRantsAsync(1);
        }

        public async Task<bool> ParseRantsAsync(int stripNumber)
        {
            IEnumerable<Strip> StripsInDatabase = await mediator.Send(new GetAllStripsQuery());

            List<Rant> rants = RantsParser.Parse(stripNumber, StripsInDatabase.Max(s => s.Number));

            IEnumerable<Rant> rantsInDatabase = await mediator.Send(new GetAllRantsQuery());

            foreach (Rant rant in rants)
            {
                if (!rantsInDatabase.Any(c => c.Number == rant.Number))
                {
                    Rant newRant = new()
                    {
                        Number = rant.Number,
                        Title = rant.Title,
                        Content = rant.Content,
                        Url = rant.Url
                    };
                    await mediator.Send(new CreateRantCommand(newRant));
                }
            }
            await mediator.Send(new SaveRantCommand());

            //IEnumerable<RantsTranslations> rantsTranslationInDatabase = await _repository.RantsTranslations.FindAllAsync();
            //foreach (Rant rant in rants)
            //{
            //    if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
            //    {
            //        RantsTranslations newTranslation = new()
            //        {
            //            Title = rant.Title,
            //            RantId = rant.RantId,
            //            Language = new CultureInfo("en-US").ToString(),
            //            Content = rant.Content
            //        };
            //        _repository.RantsTranslations.Create(newTranslation);
            //    }
            //}
            //await _repository.RantsTranslations.SaveAsync();

            return true;
        }

        public async Task<Rant> GetRantByNumber(int number)
        {
            return await mediator.Send(new GetRantQuery(number));
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Rant> rants = await mediator.Send(new GetAllRantsQuery());
            return rants.Any();
        }
    }
}
