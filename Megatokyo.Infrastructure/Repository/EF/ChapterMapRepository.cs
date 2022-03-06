using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<IEnumerable<Chapter>> GetAllAsync()
        {
            IEnumerable<ChapterEntity> chapters = await DbSet.ToListAsync();
            return Mapper.Map<IEnumerable<Chapter>>(chapters);
        }

        public async Task<Chapter> GetAsync(string category)
        {
            ChapterEntity? chapter = await DbSet.SingleOrDefaultAsync(chapter => chapter.Category == category);
            return Mapper.Map<Chapter>(chapter);
        }

        public async Task<Chapter> CreateAsync(Chapter chapterDomain)
        {
            ChapterEntity? chapterEntity = Mapper.Map<ChapterEntity>(chapterDomain);
            EntityEntry<ChapterEntity> entity = await DbSet.AddAsync(chapterEntity);
            return Mapper.Map<Chapter>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
