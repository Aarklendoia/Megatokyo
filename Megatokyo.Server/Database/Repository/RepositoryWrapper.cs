using Megatokyo.Server.Database.Contracts;

namespace Megatokyo.Server.Database.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IDbContext _repoContext;
        private IChaptersRepository _chapters;
        private IStripsRepository _strips;
        private IRantsRepository _rants;
        private IRantsTranslationsRepository _rantsTranslations;
        private ICheckingRepository _checking;

        public IChaptersRepository Chapters
        {
            get
            {
                if (_chapters == null)
                {
                    _chapters = new ChaptersRepository(_repoContext);
                }

                return _chapters;
            }
        }

        public IStripsRepository Strips
        {
            get
            {
                if (_strips == null)
                {
                    _strips = new StripsRepository(_repoContext);
                }

                return _strips;
            }
        }

        public IRantsRepository Rants
        {
            get
            {
                if (_rants == null)
                {
                    _rants = new RantsRepository(_repoContext);
                }

                return _rants;
            }
        }

        public IRantsTranslationsRepository RantsTranslations
        {
            get
            {
                if (_rantsTranslations == null)
                {
                    _rantsTranslations = new RantsTranslationsRepository(_repoContext);
                }

                return _rantsTranslations;
            }
        }

        public ICheckingRepository Checking
        {
            get
            {
                if (_checking == null)
                {
                    _checking = new CheckingRepository(_repoContext);
                }

                return _checking;
            }
        }

        public RepositoryWrapper(BackgroundDbContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}
