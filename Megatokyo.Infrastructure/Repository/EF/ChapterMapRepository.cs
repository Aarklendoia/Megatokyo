using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class ChapterMapRepository : IChapterRepository
    {
        private APIContext Context { get; }
        private IMapper Mapper { get; }
        public DbSet<ChapterEntity> DbSet { get; }

        public ChapterMapRepository(APIContext dataContext, IMapper mapper)
        {
            Context = dataContext;
            DbSet = Context.Set<ChapterEntity>();
            Mapper = mapper;
        }

        public async Task<IEnumerable<ChapterDomain>> GetAllAsync()
        {
            IEnumerable<ChapterEntity> chapters = await DbSet.ToListAsync();

            return Mapper.Map<IEnumerable<ChapterDomain>>(chapters);
        }

        public async Task<ChapterDomain> GetAsync(int number)
        {
            IEnumerable<ChapterEntity> strips =
                await DbSet.Where(strip => strip.Number == number).ToListAsync();

            return Mapper.Map<ChapterDomain>(strips);
        }
    }
}
