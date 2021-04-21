using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IChapterRepository
    {
        Task<IEnumerable<ChapterDomain>> GetAllAsync();
        Task<ChapterDomain> GetAsync(int number);
        Task<ChapterDomain> CreateAsync(ChapterDomain chapterDomain);
        Task<int> SaveAsync();
    }
}
