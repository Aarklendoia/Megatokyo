using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Megatokyo.Server.Database
{
    public class MegatokyoDbContext : DbContext
    {
        public DbSet<Chapters> Chapters { get; set; }
        public DbSet<Strips> Strips { get; set; }
        public DbSet<Rants> Rants { get; set; }
        public DbSet<RantsTranslations> RantsTranslations { get; set; }
        public DbSet<Checking> Checking { get; set; }

        public MegatokyoDbContext(DbContextOptions<MegatokyoDbContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }
    }
}
