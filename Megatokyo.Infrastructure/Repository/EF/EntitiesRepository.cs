using Megatokyo.Logic.Interfaces;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class EntitiesRepository : IEntitiesRepository
    {
        public StripMapRepository MapStrips { get; set; }

        public APIContext Context { get; }

        public IStripRepository Strips { get; set; }

        public IChapterRepository Chapters { get; set; }

        public IRantRepository Rants { get; set; }

        public EntitiesRepository(APIContext context, IStripRepository personnes)
        {
            this.Context = context;
            this.Strips = personnes;
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
