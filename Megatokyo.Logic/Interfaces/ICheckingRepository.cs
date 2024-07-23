using Megatokyo.Domain;

namespace Megatokyo.Logic.Interfaces
{
    public interface ICheckingRepository
    {
        Task<Checking> GetAsync(int number);
        Task<Checking> CreateAsync(Checking checking);
        Task<Checking> UpdateAsync(Checking checking);
    }
}
