using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<Checking> GetAsync(int number)
        {
            CheckingEntity? cheking = await DbSet.SingleOrDefaultAsync(checking => checking.Id == number);
            return Mapper.Map<Checking>(cheking);
        }

        public async Task<Checking> CreateAsync(Checking checkingDomain)
        {
            CheckingEntity checkingEntity = Mapper.Map<CheckingEntity>(checkingDomain);
            EntityEntry<CheckingEntity> entity = await DbSet.AddAsync(checkingEntity);
            await Context.SaveChangesAsync();
            return Mapper.Map<Checking>(entity.Entity);
        }

        public async Task<Checking> UpdateAsync(Checking checkingDomain)
        {
            CheckingEntity? checking = DbSet.Find(checkingDomain.Id);
            CheckingEntity? updatedChecking = Mapper.Map(checkingDomain, checking);
            await Context.SaveChangesAsync();
            return Mapper.Map<Checking>(updatedChecking);
        }
    }
}
