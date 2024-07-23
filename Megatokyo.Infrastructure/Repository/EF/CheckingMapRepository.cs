using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class CheckingMapRepository(ApiContext dataContext, IMapper mapper) : ICheckingRepository
    {
        public async Task<Checking> GetAsync(int number)
        {
            CheckingEntity? cheking = await dataContext.Checking.SingleOrDefaultAsync(checking => checking.Id == number);
            return mapper.Map<Checking>(cheking);
        }

        public async Task<Checking> CreateAsync(Checking checking)
        {
            CheckingEntity checkingEntity = mapper.Map<CheckingEntity>(checking);
            EntityEntry<CheckingEntity> entity = await dataContext.Checking.AddAsync(checkingEntity);
            await dataContext.SaveChangesAsync();
            return mapper.Map<Checking>(entity.Entity);
        }

        public async Task<Checking> UpdateAsync(Checking checking)
        {
            CheckingEntity? checkingEntity = await dataContext.Checking.FindAsync(checking.Id);
            CheckingEntity? updatedCheckingEntity = mapper.Map(checking, checkingEntity);
            await dataContext.SaveChangesAsync();
            return mapper.Map<Checking>(updatedCheckingEntity);
        }
    }
}
