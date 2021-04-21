using Megatokyo.Infrastructure.Repository.EF.Entity;
using Microsoft.EntityFrameworkCore;

namespace Megatokyo.Infrastructure.Repository.Entity
{
    class HostedContext : DbContext
    {
        public DbSet<ChapterEntity> Chapters { get; set; }
        public DbSet<StripEntity> Strips { get; set; }
        public DbSet<RantEntity> Rants { get; set; }

        public HostedContext()
        {
        }

        public HostedContext(DbContextOptions<HostedContext> options) : base(options)
        {
        }
    }
}
