using Megatokyo.Server.Models.Translations;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Megatokyo.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Megatokyo.Server.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            services.AddDbContext<ApiDbContext>(options =>
            {
                var connectionString = new SqliteConnectionStringBuilder(configuration.GetConnectionString("SqliteConnection"))
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ToString();
                options.UseSqlite(connectionString);
            });
            services.AddDbContext<BackgroundDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
            }, ServiceLifetime.Singleton);
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureTranslator(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string translatorKey = configuration["BingTranslator:ClientKey"];
            services.AddScoped<ITranslator, Translator>();
            services.Configure<TranslatorSettings>(settings => settings.ClientKey = translatorKey);
        }
    }
}
