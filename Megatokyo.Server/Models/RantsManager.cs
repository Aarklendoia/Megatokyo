using Megatokyo.Server.Database;
using Megatokyo.Server.Database.Models;
using Megatokyo.Server.Database.Repository;
using Megatokyo.Server.Models.Entities;
using Megatokyo.Server.Models.Parsers;
using Microsoft.Extensions.Configuration;
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
        public RantsManager(MegatokyoDbContext megatokyoDbContext)
        {
            _repositoryContext = megatokyoDbContext;
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
        public async Task<bool> ParseRantsAsync()
        {
            return await ParseRantsAsync(1);
        }

        /// <summary>
        /// Extrait les diatribes puis les stocke en base de données.
        /// </summary>
        /// <param name="stripNumber">Dernière planche extraite à partir de laquelle chercher une nouvelle diatribe.</param>
        /// <returns></returns>
        public async Task<bool> ParseRantsAsync(int stripNumber)
        {
            IEnumerable<Strips> StripsInDatabase = await _repository.Strips.FindAllAsync();

            List<Rant> rants = RantsParser.Parse(stripNumber, StripsInDatabase.Max(s => s.Number));

            IEnumerable<Rants> rantsInDatabase = await _repository.Rants.FindAllAsync();

            foreach (Rant rant in rants)
            {
                if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
                {
                    Rants newRant = new()
                    {
                        Number = rant.Number,
                        Date = rant.DateTime,
                        Author = rant.Author
                    };
                    _repository.Rants.Create(newRant);
                    await _repository.Rants.SaveAsync();
                    rant.RantId = newRant.RantId;
                }
            }

            IEnumerable<RantsTranslations> rantsTranslationInDatabase = await _repository.RantsTranslations.FindAllAsync();
            foreach (Rant rant in rants)
            {
                if (!rantsInDatabase.Where(c => c.Number == rant.Number).Any())
                {
                    RantsTranslations newTranslation = new()
                    {
                        Title = rant.Title,
                        RantId = rant.RantId,
                        Language = new CultureInfo("en-US").ToString(),
                        Content = rant.Content
                    };
                    _repository.RantsTranslations.Create(newTranslation);
                }
            }
            await _repository.RantsTranslations.SaveAsync();

            return true;
        }

        public async Task<Rant> GetRantByNumber(int number)
        {
            IEnumerable<Rants> rants = await _repository.Rants.FindByConditionAsync(s => s.Number == number);
            return new Rant(rants.First());
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Rants> rants = await _repository.Rants.FindAllAsync();
            return rants.Any();
        }
    }
}
