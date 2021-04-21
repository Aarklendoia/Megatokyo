using Megatokyo.Infrastructure.Repository.EF;
using Megatokyo.Logic.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Megatokyo.Infrastructure
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<APIContext>(options =>
            {
                var connectionString = new SqliteConnectionStringBuilder(configuration.GetConnectionString("SqliteConnection"))
                {
                    Mode = SqliteOpenMode.ReadOnly,
                }.ToString();
                options.UseSqlite(connectionString);
            });

            //services.AddDbContext<BackgroundDbContext>(options =>
            //{
            //    options.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
            //}, ServiceLifetime.Singleton);

            services.AddScoped<IEntitiesRepository, EntitiesRepository>();
            services.AddScoped<IStripRepository, StripMapRepository>();
            services.AddScoped<StripMapRepository>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //string translatorKey = configuration["BingTranslator:ClientKey"];
            //services.AddScoped<ITranslator, Translator>();
            //services.Configure<TranslatorSettings>(settings => settings.ClientKey = translatorKey);

            return services;
        }

    }
}
