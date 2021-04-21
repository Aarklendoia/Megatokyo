using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TP.Logic
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
