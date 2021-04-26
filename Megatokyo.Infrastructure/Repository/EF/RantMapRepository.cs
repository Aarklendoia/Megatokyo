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
    public class RantMapRepository : IRantRepository
    {
        private APIContext Context { get; }
        private IMapper Mapper { get; }
        public DbSet<RantEntity> DbSet { get; }

        public RantMapRepository(APIContext dataContext, IMapper mapper)
        {
            Context = dataContext;
            DbSet = Context.Set<RantEntity>();
            Mapper = mapper;
        }

        public async Task<IEnumerable<RantDomain>> GetAllAsync()
        {
            IEnumerable<RantEntity> rants = await DbSet.ToListAsync();
            return Mapper.Map<IEnumerable<RantDomain>>(rants);
        }

        public async Task<RantDomain> GetAsync(int number)
        {            
            RantEntity rant = await DbSet.SingleOrDefaultAsync(rant => rant.Number == number);
            return Mapper.Map<RantDomain>(rant);
        }

        public async Task<RantDomain> CreateAsync(RantDomain rantDomain)
        {
            RantEntity rantEntity = Mapper.Map<RantEntity>(rantDomain);
            EntityEntry<RantEntity> entity = await DbSet.AddAsync(rantEntity);
            return Mapper.Map<RantDomain>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
