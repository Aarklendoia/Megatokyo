using Megatokyo.Infrastructure.Repository.EF.Entity;
using Microsoft.EntityFrameworkCore;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class APIContext : DbContext
    {
        public DbSet<ChapterEntity> Chapters { get; set; }
        public DbSet<StripEntity> Strips { get; set; }
        public DbSet<RantEntity> Rants { get; set; }
        //public DbSet<RantsTranslations> RantsTranslations { get; set; }
        //public DbSet<Checking> Checking { get; set; }

        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }
    }
}
