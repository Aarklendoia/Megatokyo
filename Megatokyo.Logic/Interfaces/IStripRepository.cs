using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IStripRepository
    {
        Task<IEnumerable<StripDomain>> GetAllAsync();
        Task<StripDomain> GetAsync(int number);
    }
}
