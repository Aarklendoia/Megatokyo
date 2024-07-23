using Megatokyo.Domain;

namespace Megatokyo.Logic.Interfaces
{
    public interface IChapterRepository
    {
        Task<IEnumerable<Chapter>> GetAllAsync();
        Task<Chapter> GetAsync(string category);
        Task<Chapter> CreateAsync(Chapter chapter);
        Task<int> SaveAsync();
    }
}
