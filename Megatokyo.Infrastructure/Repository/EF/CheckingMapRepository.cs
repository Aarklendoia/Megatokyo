using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class CheckingMapRepository : ICheckingRepository
    {
        private APIContext Context { get; }
        private IMapper Mapper { get; }
        public DbSet<CheckingEntity> DbSet { get; }

        public CheckingMapRepository(APIContext dataContext, IMapper mapper)
        {
            Context = dataContext;
            DbSet = Context.Set<CheckingEntity>();
            Mapper = mapper;
        }

        public async Task<CheckingDomain> GetAsync(int number)
        {
            CheckingEntity cheking = await DbSet.SingleOrDefaultAsync(checking => checking.Id == number);
            return Mapper.Map<CheckingDomain>(cheking);
        }

        public async Task<CheckingDomain> CreateAsync(CheckingDomain checkingDomain)
        {
            CheckingEntity checkingEntity = Mapper.Map<CheckingEntity>(checkingDomain);
            EntityEntry<CheckingEntity> entity = await DbSet.AddAsync(checkingEntity);
            await Context.SaveChangesAsync();
            return Mapper.Map<CheckingDomain>(entity.Entity);
        }

        public async Task<CheckingDomain> UpdateAsync(CheckingDomain checkingDomain)
        {
            CheckingEntity checking = DbSet.Find(checkingDomain.Id);
            CheckingEntity updatedChecking = Mapper.Map(checkingDomain, checking);
            await Context.SaveChangesAsync();
            return Mapper.Map<CheckingDomain>(updatedChecking);
        }
    }
}
