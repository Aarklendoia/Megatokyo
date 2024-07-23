/*
 * Migration :
 * dotnet ef migrations add InitialCreate --project .\Megatokyo.Infrastructure\Megatokyo.Infrastructure.csproj --startup-project .\Megatokyo.Server\Megatokyo.Server.csproj
 * 
 * Mise à jour :
 * dotnet ef database update --project .\Megatokyo.Infrastructure\Megatokyo.Infrastructure.csproj --startup-project .\Megatokyo.Server\Megatokyo.Server.csproj
 * 
 * Création du bundle :
 * dotnet ef migrations bundle --project .\Megatokyo.Infrastructure\Megatokyo.Infrastructure.csproj --startup-project .\Megatokyo.Server\Megatokyo.Server.csproj
 */

using Megatokyo.Infrastructure.Repository.EF.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Megatokyo.Infrastructure.Repository.EF
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApiContext>
    {
        public ApiContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiContext>();
            optionsBuilder.UseSqlite(new SqliteConnectionStringBuilder("FileName=Megatokyo.db").ToString());

            return new ApiContext(optionsBuilder.Options);
        }
    }

    public class ApiContext(DbContextOptions<ApiContext> options) : DbContext(options)
    {
        public DbSet<ChapterEntity> Chapters { get; set; } = null!;
        public DbSet<StripEntity> Strips { get; set; } = null!;
        public DbSet<RantEntity> Rants { get; set; } = null!;
        //public DbSet<RantsTranslations> RantsTranslations { get; set; }
        public DbSet<CheckingEntity> Checking { get; set; } = null!;
    }
}


