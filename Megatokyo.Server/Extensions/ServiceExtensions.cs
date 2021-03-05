using Megatokyo.Server.Models.Translations;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Megatokyo.Server.Database;

namespace Megatokyo.Server.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            services.AddDbContext<MegatokyoDbContext>();
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void ConfigureTranslator(this IServiceCollection services, IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            string translatorKey = config["BingTranslator:ClientKey"];
            services.AddScoped<ITranslator, Translator>();
            services.Configure<TranslatorSettings>(settings => settings.ClientKey = translatorKey);
        }
    }
}
