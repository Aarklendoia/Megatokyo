using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IChapterRepository
    {
        Task<IEnumerable<Chapter>> GetAllAsync();
        Task<Chapter> GetAsync(string category);
        Task<Chapter> CreateAsync(Chapter chapterDomain);
        Task<int> SaveAsync();
    }
}
