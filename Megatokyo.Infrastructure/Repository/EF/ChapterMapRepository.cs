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
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public ChapterMapRepository(APIContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Chapter>> GetAllAsync()
        {
            IEnumerable<ChapterEntity> chapters = await _context.Chapters.ToListAsync();
            return _mapper.Map<IEnumerable<Chapter>>(chapters);
        }

        public async Task<Chapter> GetAsync(string category)
        {
            ChapterEntity? chapter = await _context.Chapters.SingleOrDefaultAsync(chapter => chapter.Category == category);
            return _mapper.Map<Chapter>(chapter);
        }

        public async Task<Chapter> CreateAsync(Chapter chapterDomain)
        {
            ChapterEntity? chapterEntity = _mapper.Map<ChapterEntity>(chapterDomain);
            EntityEntry<ChapterEntity> entity = await _context.Chapters.AddAsync(chapterEntity);
            return _mapper.Map<Chapter>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
