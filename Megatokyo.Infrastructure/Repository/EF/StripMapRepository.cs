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

        public async Task<IEnumerable<Strip>> GetAllAsync()
        {
            IEnumerable<StripEntity> strips = await DbSet.ToListAsync();
            return Mapper.Map<IEnumerable<Strip>>(strips);
        }

        public async Task<Strip> GetAsync(int number)
        {
            StripEntity strip = await DbSet.SingleOrDefaultAsync(strip => strip.Number == number);
            return Mapper.Map<Strip>(strip);
        }

        public async Task<Strip> CreateAsync(Strip stripDomain)
        {
            StripEntity stripEntity = Mapper.Map<StripEntity>(stripDomain);
            EntityEntry<StripEntity> entity = await DbSet.AddAsync(stripEntity);
            return Mapper.Map<Strip>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Strip>> GetCategoryAsync(string category)
        {
            IEnumerable<StripEntity> strips = await DbSet.Where(strip => strip.Category == category).ToListAsync();
            return Mapper.Map<IEnumerable<Strip>>(strips);
        }
    }
}
