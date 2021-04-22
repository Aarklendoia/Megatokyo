using Megatokyo.Domain;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface ICheckingRepository
    {
        Task<CheckingDomain> GetAsync(int number);
        Task<CheckingDomain> CreateAsync(CheckingDomain checkingDomain);
        Task<CheckingDomain> UpdateAsync(CheckingDomain checkingDomain);
    }
}
