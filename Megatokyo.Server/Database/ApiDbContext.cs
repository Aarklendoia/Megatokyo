using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Megatokyo.Server.Database
{
    public class ApiDbContext : DbContext, IDbContext
    {
        public DbSet<Chapters> Chapters { get; set; }
        public DbSet<Strips> Strips { get; set; }
        public DbSet<Rants> Rants { get; set; }
        public DbSet<RantsTranslations> RantsTranslations { get; set; }
        public DbSet<Checking> Checking { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }
    }
}
