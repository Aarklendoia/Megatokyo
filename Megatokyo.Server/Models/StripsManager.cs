using Megatokyo.Server.Database;
using Megatokyo.Server.Database.Models;
using Megatokyo.Server.Database.Repository;
using Megatokyo.Server.Models.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    internal class StripsManager: IDisposable
    {
        private readonly MegatokyoDbContext _repositoryContext;
        private readonly RepositoryWrapper _repository;
        

        public Uri Url { get; set; }

        /// <summary>
        /// Extrait du site de Megatokyo les chapitres et les planches puis les stocke en base de données.
        /// </summary>
        /// <param name="url">URL de la page d'archives de Megatokyo.</param>
        public StripsManager(Uri url)
        {
            Url = url;
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
        /// Extrait les chapitres puis les stocke en base de données.
        /// </summary>
        public async Task<IList<Chapter>> ParseChaptersAsync()
        {
            IList<Chapter> chapters = ChaptersParser.Parse(Url);

            IEnumerable<Chapters> chaptersInDatabase = await _repository.Chapters.FindAllAsync();

            foreach (Chapter chapter in chapters)
            {
                if (!chaptersInDatabase.Where(c => c.Number == chapter.Number).Any())
                {
                    Chapters newChapter = new Chapters
                    {
                        Category = chapter.Category,
                        Number = chapter.Number,
                        Title = chapter.Title
                    };
                    _repository.Chapters.Create(newChapter);
                }
            }
            await _repository.Chapters.SaveAsync();
            return chapters;
        }

        /// <summary>
        /// Recherches toutes les planches des chapitres fournis.
        /// </summary>
        /// <param name="chapters">Liste des chapitres pour lesquels il faut rechercher les planches.</param>
        /// <returns></returns>
        public async Task ParseStripsAsync(IList<Chapter> chapters)
        {
            IEnumerable<Strips> stripsInDatabase = await _repository.Strips.FindAllAsync();

            List<Strip> strips = await StripsParser.ParseAsync(Url, chapters, stripsInDatabase);

            IEnumerable<Chapters> chaptersInDatabase = await _repository.Chapters.FindAllAsync();
            foreach (Strip strip in strips)
            {
                if (!stripsInDatabase.Where(s => s.Number == strip.Number).Any())
                {
                    Chapters currentChapter = chaptersInDatabase.Where(c => c.Category == strip.Category).First();
                    Strips newStrip = new Strips
                    {
                        Title = strip.Title,
                        Number = strip.Number,
                        ChapterId = currentChapter.ChapterId,
                        Url = strip.Url,
                        Date = strip.Date
                    };
                    _repository.Strips.Create(newStrip);
                }
            }
            await _repository.Strips.SaveAsync();
        }

        public async Task<DetailedStrip> GetStripByNumberAsync(int number)
        {
            IEnumerable<Strips> strips = await _repository.Strips.FindByConditionAsync(s => s.Number == number);
            return new DetailedStrip(strips.First());
        }

        public async Task<bool> CheckIfDataExistsAsync()
        {
            IEnumerable<Strips> strips = await _repository.Strips.FindAllAsync();
            return strips.Any();
        }
    }
}
