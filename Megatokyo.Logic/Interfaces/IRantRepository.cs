using Megatokyo.Domain;

namespace Megatokyo.Logic.Interfaces
{
    public interface IRantRepository
    {
        Task<IEnumerable<Rant>> GetAllAsync();
        Task<Rant> GetAsync(int number);
        Task<Rant> CreateAsync(Rant rantDomain);
        Task<int> SaveAsync();
    }
}
