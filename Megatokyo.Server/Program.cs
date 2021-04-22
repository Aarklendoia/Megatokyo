using Megatokyo.Server.Models.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Megatokyo.Server
{
    /// <summary>
    /// Main program of the webAPI.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ConsumeScopedServiceHostedService>();
                services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            });
    }
}
