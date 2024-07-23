using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class StripMapRepository(ApiContext dataContext, IMapper mapper) : IStripRepository
    {
        public async Task<IEnumerable<Strip>> GetAllAsync()
        {
            IEnumerable<StripEntity> strips = await dataContext.Strips.ToListAsync();
            return mapper.Map<IEnumerable<Strip>>(strips);
        }

        public async Task<Strip> GetAsync(int number)
        {
            StripEntity? strip = await dataContext.Strips.SingleOrDefaultAsync(strip => strip.Number == number);
            return mapper.Map<Strip>(strip);
        }

        public async Task<Strip> CreateAsync(Strip strip)
        {
            StripEntity stripEntity = mapper.Map<StripEntity>(strip);
            EntityEntry<StripEntity> entity = await dataContext.Strips.AddAsync(stripEntity);
            return mapper.Map<Strip>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Strip>> GetCategoryAsync(string category)
        {
            IEnumerable<StripEntity> strips = await dataContext.Strips.Where(strip => strip.Category == category).ToListAsync();
            return mapper.Map<IEnumerable<Strip>>(strips);
        }
    }
}
