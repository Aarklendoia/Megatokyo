using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Megatokyo.Server.Database.Repository
{
    public class StripsRepository : RepositoryBase<Strips>, IStripsRepository
    {
        public StripsRepository(MegatokyoDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public override async Task<IEnumerable<Strips>> FindAllAsync()
        {
            return await RepositoryContext.Set<Strips>().OrderBy(s => s.Number).ToListAsync();
        }
    }
}