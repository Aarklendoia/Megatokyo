using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Commands;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Models.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    internal class RantsManager
    {
        private readonly IMediator _mediator;

        public RantsManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> ParseRantsAsync()
        {
            return await ParseRantsAsync(1);
        }

        public async Task<bool> ParseRantsAsync(int stripNumber)
        {
            IEnumerable<StripDomain> StripsInDatabase = await _mediator.Send(new GetAllStripsQuery());

            List<RantDomain> rants = RantsParser.Parse(stripNumber, StripsInDatabase.Max(s => s.Number));

            IEnumerable<RantDomain> rantsInDatabase = await _mediator.Send(new GetAllRantsQuery());

            foreach (RantDomain rant in rants)
            {
                if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
                {
                    RantDomain newRant = new(rant.Title, rant.Number, rant.Author, rant.Url, rant.Timestamp, rant.Content);
                    await _mediator.Send(new CreateRantCommand(newRant));
                }
            }
            await _mediator.Send(new SaveRantCommand());

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

        public async Task<RantDomain> GetRantByNumber(int number)
        {
            return await _mediator.Send(new GetRantQuery(number));
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<RantDomain> rants = await _mediator.Send(new GetAllRantsQuery());
            return rants.Any();
        }
    }
}
