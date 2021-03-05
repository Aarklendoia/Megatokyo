using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Database.Repository
{
    public class ChaptersRepository : RepositoryBase<Chapters>, IChaptersRepository
    {
        public ChaptersRepository(MegatokyoDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public override async Task<IEnumerable<Chapters>> FindAllAsync()
        {
            return await RepositoryContext.Set<Chapters>().OrderBy(c => c.Number).ToListAsync();
        }

        public async Task<IEnumerable<Chapters>> FindAllStoryChapters()
        {
            return await RepositoryContext.Set<Chapters>().Where(c => c.Category.StartsWith("C-", System.StringComparison.InvariantCulture)).OrderBy(c => c.Number).ToListAsync();
        }
    }
}