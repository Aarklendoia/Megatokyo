using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class StripMapRepository : IStripRepository
    {
        private APIContext Context { get; }
        private IMapper Mapper { get; }
        public DbSet<StripEntity> DbSet { get; }

        public StripMapRepository(APIContext dataContext, IMapper mapper)
        {
            Context = dataContext;
            DbSet = Context.Set<StripEntity>();
            Mapper = mapper;
        }

        public async Task<IEnumerable<StripDomain>> GetAllAsync()
        {
            IEnumerable<StripEntity> strips =
                await DbSet.Include(strip => strip.Chapter).ToListAsync();

            return Mapper.Map<IEnumerable<StripDomain>>(strips);
        }

        public async Task<StripDomain> GetAsync(int number)
        {
            IEnumerable<StripEntity> strips =
                await DbSet.Where(strip => strip.Number == number).Include(strip => strip.Chapter).ToListAsync();

            return Mapper.Map<StripDomain>(strips);
        }

        public async Task<StripDomain> CreateAsync(StripDomain stripDomain)
        {
            StripEntity stripEntity = Mapper.Map<StripEntity>(stripDomain);
            EntityEntry<StripEntity> entity = await DbSet.AddAsync(stripEntity);
            return Mapper.Map<StripDomain>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
