using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;

namespace Megatokyo.Server.Database.Repository
{
    public class RantsTranslationsRepository : RepositoryBase<RantsTranslations>, IRantsTranslationsRepository
    {
        public RantsTranslationsRepository(MegatokyoDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
    }
}
