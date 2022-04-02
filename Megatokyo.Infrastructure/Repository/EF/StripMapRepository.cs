using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class StripMapRepository : IStripRepository
    {
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public StripMapRepository(APIContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Strip>> GetAllAsync()
        {
            IEnumerable<StripEntity> strips = await _context.Strips.ToListAsync();
            return _mapper.Map<IEnumerable<Strip>>(strips);
        }

        public async Task<Strip> GetAsync(int number)
        {
            StripEntity? strip = await _context.Strips.SingleOrDefaultAsync(strip => strip.Number == number);
            return _mapper.Map<Strip>(strip);
        }

        public async Task<Strip> CreateAsync(Strip stripDomain)
        {
            StripEntity stripEntity = _mapper.Map<StripEntity>(stripDomain);
            EntityEntry<StripEntity> entity = await _context.Strips.AddAsync(stripEntity);
            return _mapper.Map<Strip>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Strip>> GetCategoryAsync(string category)
        {
            IEnumerable<StripEntity> strips = await _context.Strips.Where(strip => strip.Category == category).ToListAsync();
            return _mapper.Map<IEnumerable<Strip>>(strips);
        }
    }
}
