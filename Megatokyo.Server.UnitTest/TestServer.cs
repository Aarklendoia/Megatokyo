using Megatokyo.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace EIG.Formation.ClientAPI.UnitTest
{
    [TestClass]
    public class TestServer
    {
        private static string BaseAddress { get; } = "https://localhost/";
        private static Startup Startup { get; set; }
        public static IHost HostTest { get; private set; }

        [AssemblyInitialize()]
        public static void Init(TestContext context)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Startup = new Startup(config);
            var path = Assembly.GetEntryAssembly().Location;

            var hostBuilder = new HostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureServices(services => ConfigureServices(services))
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();
                    webHost.Configure((ctxt, app) => Startup.Configure(app, ctxt.HostingEnvironment));
                })
                ;
            HostTest = hostBuilder.Build();
            HostTest.Start();
        }

        public static void ConfigureServices(IServiceCollection services)
        {

            Startup.ConfigureServices(services);
            services.AddControllers()
                .AddApplicationPart(typeof(Startup).Assembly);
            services.AddHttpClient("", build =>
            {
                build.BaseAddress = new Uri(BaseAddress);
                build.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            ).
            ConfigurePrimaryHttpMessageHandler(_ => HostTest.GetTestServer().CreateHandler());

        }

        public static HttpClient GetClient()
        {
            return HostTest.GetTestClient();
        }
    }
}
