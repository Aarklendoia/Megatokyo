using Megatokyo.Server.Database;
using Megatokyo.Server.Database.Models;
using Megatokyo.Server.Database.Repository;
using Megatokyo.Server.Models.Entities;
using Megatokyo.Server.Models.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    internal class RantsManager : IDisposable
    {
        private readonly MegatokyoDbContext _repositoryContext;
        private readonly RepositoryWrapper _repository;

        /// <summary>
        /// Extrait du site de Megatokyo les diatribes puis les stocke en base de données.
        /// </summary>
        public RantsManager()
        {
            _repositoryContext = new MegatokyoDbContext();
            _repository = new RepositoryWrapper(_repositoryContext);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repositoryContext.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Extrait les diatribes puis les stocke en base de données.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Rant>> ParseRantsAsync()
        {
            return await ParseRantsAsync(1).ConfigureAwait(false);
        }

        /// <summary>
        /// Extrait les diatribes puis les stocke en base de données.
        /// </summary>
        /// <param name="stripNumber">Dernière planche extraite à partir de laquelle chercher une nouvelle diatribe.</param>
        /// <returns></returns>
        public async Task<IList<Rant>> ParseRantsAsync(int stripNumber)
        {
            IEnumerable<Strips> StripsInDatabase = await _repository.Strips.FindAllAsync().ConfigureAwait(false);

            List<Rant> rants = RantsParser.Parse(stripNumber, StripsInDatabase.Max(s => s.Number));

            IEnumerable<Rants> rantsInDatabase = await _repository.Rants.FindAllAsync().ConfigureAwait(false);

            foreach (Rant rant in rants)
            {
                if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
                {
                    Rants newRant = new Rants
                    {
                        Number = rant.Number,
                        Date = rant.DateTime,
                        Author = rant.Author
                    };
                    _repository.Rants.Create(newRant);
                    await _repository.Rants.SaveAsync().ConfigureAwait(false);
                    rant.RantId = newRant.RantId;
                }
            }

            IEnumerable<RantsTranslations> rantsTranslationInDatabase = await _repository.RantsTranslations.FindAllAsync().ConfigureAwait(false);
            foreach (Rant rant in rants)
            {
                if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
                {
                    RantsTranslations newTranslation = new RantsTranslations
                    {
                        Title = rant.Title,
                        RantId = rant.RantId,
                        Language = new CultureInfo("en-US").ToString(),
                        Content = rant.Content
                    };
                    _repository.RantsTranslations.Create(newTranslation);
                }
            }
            await _repository.RantsTranslations.SaveAsync().ConfigureAwait(false);

            return rants;
        }

        public async Task<Rant> GetRantByNumber(int number)
        {
            IEnumerable<Rants> rants = await _repository.Rants.FindByConditionAsync(s => s.Number == number).ConfigureAwait(false);
            return new Rant(rants.First());
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Rants> rants = await _repository.Rants.FindAllAsync().ConfigureAwait(false);
            return rants.Any();
        }
    }
}
