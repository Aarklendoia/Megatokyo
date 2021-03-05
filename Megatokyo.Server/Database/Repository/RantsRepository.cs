using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Database.Repository
{
    public class RantsRepository : RepositoryBase<Rants>, IRantsRepository
    {
        public RantsRepository(MegatokyoDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public override async Task<IEnumerable<Rants>> FindAllAsync()
        {
            return await RepositoryContext.Set<Rants>().OrderByDescending(r => r.Number).ToListAsync();
        }
    }
}