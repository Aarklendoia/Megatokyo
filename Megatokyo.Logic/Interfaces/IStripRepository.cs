using Megatokyo.Domain;

namespace Megatokyo.Logic.Interfaces
{
    public interface IStripRepository
    {
        Task<IEnumerable<Strip>> GetAllAsync();
        Task<IEnumerable<Strip>> GetCategoryAsync(string category);
        Task<Strip> GetAsync(int number);
        Task<Strip> CreateAsync(Strip strip);
        Task<int> SaveAsync();
    }
}
