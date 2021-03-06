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
                var connectionString = new SqliteConnectionStringBuilder(configuration.GetConnectionString("SqliteConnection")).ToString();
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IStripRepository, StripMapRepository>();
            services.AddScoped<StripMapRepository>();

            services.AddScoped<IChapterRepository, ChapterMapRepository>();
            services.AddScoped<ChapterMapRepository>();

            services.AddScoped<IRantRepository, RantMapRepository>();
            services.AddScoped<RantMapRepository>();

            services.AddScoped<ICheckingRepository, CheckingMapRepository>();
            services.AddScoped<CheckingMapRepository>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //string translatorKey = configuration["BingTranslator:ClientKey"];
            //services.AddScoped<ITranslator, Translator>();
            //services.Configure<TranslatorSettings>(settings => settings.ClientKey = translatorKey);

            return services;
        }

    }
}
