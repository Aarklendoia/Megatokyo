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
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public CheckingMapRepository(APIContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<Checking> GetAsync(int number)
        {
            CheckingEntity? cheking = await _context.Checking.SingleOrDefaultAsync(checking => checking.Id == number);
            return _mapper.Map<Checking>(cheking);
        }

        public async Task<Checking> CreateAsync(Checking checkingDomain)
        {
            CheckingEntity checkingEntity = _mapper.Map<CheckingEntity>(checkingDomain);
            EntityEntry<CheckingEntity> entity = await _context.Checking.AddAsync(checkingEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Checking>(entity.Entity);
        }

        public async Task<Checking> UpdateAsync(Checking checkingDomain)
        {
            CheckingEntity? checking = _context.Checking.Find(checkingDomain.Id);
            CheckingEntity? updatedChecking = _mapper.Map(checkingDomain, checking);
            await _context.SaveChangesAsync();
            return _mapper.Map<Checking>(updatedChecking);
        }
    }
}
