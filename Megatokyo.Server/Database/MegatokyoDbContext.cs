using Megatokyo.Server.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public DbSet<Chapters> Chapters { get; set; }
        public DbSet<Strips> Strips { get; set; }
        public DbSet<Rants> Rants { get; set; }
        public DbSet<RantsTranslations> RantsTranslations { get; set; }
        public DbSet<Checking> Checking { get; set; }

        public MegatokyoDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            string connectionString = _configuration.GetConnectionString("SqliteConnection");

            optionsBuilder.UseSqlite(connectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
    }
}
