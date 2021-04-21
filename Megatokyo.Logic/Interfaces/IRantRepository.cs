using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IRantRepository
    {
        Task<int> SaveAsync();
        Task<IEnumerable<RantDomain>> GetAllAsync();
    }
}
