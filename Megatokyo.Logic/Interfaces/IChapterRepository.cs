using Megatokyo.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IChapterRepository
    {
        Task<int> SaveAsync();
        Task<IEnumerable<ChapterDomain>> GetAllAsync();
    }
}
