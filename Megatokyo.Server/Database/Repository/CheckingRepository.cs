using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;

namespace Megatokyo.Server.Database.Repository
{
    public class CheckingRepository : RepositoryBase<Checking>, ICheckingRepository
    {
        public CheckingRepository(IDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
    }
}