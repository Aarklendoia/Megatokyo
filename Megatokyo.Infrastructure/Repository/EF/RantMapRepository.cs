using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class RantMapRepository(ApiContext dataContext, IMapper mapper) : IRantRepository
    {
        public async Task<IEnumerable<Rant>> GetAllAsync()
        {
            IEnumerable<RantEntity> rants = await dataContext.Rants.ToListAsync();
            return mapper.Map<IEnumerable<Rant>>(rants);
        }

        public async Task<Rant> GetAsync(int number)
        {
            RantEntity? rant = await dataContext.Rants.SingleOrDefaultAsync(rant => rant.Number == number);
            return mapper.Map<Rant>(rant);
        }

        public async Task<Rant> CreateAsync(Rant rant)
        {
            RantEntity? rantEntity = mapper.Map<RantEntity>(rant);
            EntityEntry<RantEntity> entity = await dataContext.Rants.AddAsync(rantEntity);
            return mapper.Map<Rant>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await dataContext.SaveChangesAsync();
        }
    }
}
