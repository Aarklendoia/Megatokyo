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
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public RantMapRepository(APIContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Rant>> GetAllAsync()
        {
            IEnumerable<RantEntity> rants = await _context.Rants.ToListAsync();
            return _mapper.Map<IEnumerable<Rant>>(rants);
        }

        public async Task<Rant> GetAsync(int number)
        {
            RantEntity? rant = await _context.Rants.SingleOrDefaultAsync(rant => rant.Number == number);
            return _mapper.Map<Rant>(rant);
        }

        public async Task<Rant> CreateAsync(Rant rantDomain)
        {
            RantEntity? rantEntity = _mapper.Map<RantEntity>(rantDomain);
            EntityEntry<RantEntity> entity = await _context.Rants.AddAsync(rantEntity);
            return _mapper.Map<Rant>(entity.Entity);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
