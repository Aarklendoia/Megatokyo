using System.Threading.Tasks;

namespace Megatokyo.Logic.Interfaces
{
    public interface IEntitiesRepository
    {
        Task<int> SaveAsync();
        IChapterRepository Chapters { get; }
        IStripRepository Strips { get; }
        IRantRepository Rants { get; }
    }
}
