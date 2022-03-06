using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<IEnumerable<Rant>> GetAllAsync()
        {
            IEnumerable<RantEntity> rants = await DbSet.ToListAsync();
            return Mapper.Map<IEnumerable<Rant>>(rants);
        }

        public async Task<Rant> GetAsync(int number)
        {
            RantEntity? rant = await DbSet.SingleOrDefaultAsync(rant => rant.Number == number);
            return Mapper.Map<Rant>(rant);
        }

        public async Task<Rant> CreateAsync(Rant rantDomain)
        {
            RantEntity? rantEntity = Mapper.Map<RantEntity>(rantDomain);
            EntityEntry<RantEntity> entity = await DbSet.AddAsync(rantEntity);
            return Mapper.Map<Rant>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
