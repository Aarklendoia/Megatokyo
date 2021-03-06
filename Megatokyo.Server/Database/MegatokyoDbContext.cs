using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Megatokyo.Server.Database
{
    public class MegatokyoDbContext : DbContext
    {
        public DbSet<Chapters> Chapters { get; set; }
        public DbSet<Strips> Strips { get; set; }
        public DbSet<Rants> Rants { get; set; }
        public DbSet<RantsTranslations> RantsTranslations { get; set; }
        public DbSet<Checking> Checking { get; set; }

        public MegatokyoDbContext()
        {
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            optionsBuilder.UseSqlite("Filename=" + assembly.Location.Replace(".dll", ".db", true, CultureInfo.CurrentCulture), options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
    }
}
