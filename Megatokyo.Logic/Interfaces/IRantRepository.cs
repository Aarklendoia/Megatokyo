using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IRantRepository
    {
        Task<IEnumerable<RantDomain>> GetAllAsync();
        Task<RantDomain> GetAsync(int number);
        Task<RantDomain> CreateAsync(RantDomain rantDomain);
        Task<int> SaveAsync();
    }
}
