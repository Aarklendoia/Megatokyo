using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TP.Logic
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
            return services;
        }
    }
}
