using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class ChapterMapRepository(ApiContext dataContext, IMapper mapper) : IChapterRepository
    {
        public async Task<IEnumerable<Chapter>> GetAllAsync()
        {
            IEnumerable<ChapterEntity> chapters = await dataContext.Chapters.ToListAsync();
            return mapper.Map<IEnumerable<Chapter>>(chapters);
        }

        public async Task<Chapter> GetAsync(string category)
        {
            ChapterEntity? chapter = await dataContext.Chapters.SingleOrDefaultAsync(chapter => chapter.Category == category);
            return mapper.Map<Chapter>(chapter);
        }

        public async Task<Chapter> CreateAsync(Chapter chapter)
        {
            ChapterEntity? chapterEntity = mapper.Map<ChapterEntity>(chapter);
            EntityEntry<ChapterEntity> entity = await dataContext.Chapters.AddAsync(chapterEntity);
            return mapper.Map<Chapter>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await dataContext.SaveChangesAsync();
        }
    }
}
