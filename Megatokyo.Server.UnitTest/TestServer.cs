using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Megatokyo.Infrastructure;
using Megatokyo.Infrastructure.Repository.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text.Json.Serialization;
using TP.Logic;

namespace IG.MaRH.ClientAPI.UnitTest.Server
{
    [TestClass]
    public class TestServer
    {
        private static SqliteConnection _keepAliveConnection;
        private static IHost HostTest;
        private static APIContext _context;

        [AssemblyInitialize()]
#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public static void Init(TestContext context)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            string path = Assembly.GetEntryAssembly().Location;

            IHostBuilder hostBuilder = new HostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure((ctxt, app) => Configure(app, ctxt.HostingEnvironment));
                })
                .ConfigureServices(services => ConfigureServices(services));
            HostTest = hostBuilder.Build();
            HostTest.Start();
        }

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        private static void Configure(IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Utilisation d'une base en mémoire
            _keepAliveConnection = new("DataSource=:memory:");
            _keepAliveConnection.Open();

            services.AddDbContext<APIContext>(options =>
            {
                DbContextOptionsBuilder contextOptionsBuilder = options.UseSqlite(_keepAliveConnection);
                contextOptionsBuilder.EnableSensitiveDataLogging(true);
                DbContextOptionsBuilder<APIContext> dbContextOptionsBuilder = (DbContextOptionsBuilder<APIContext>)contextOptionsBuilder;
                _context = new APIContext(dbContextOptionsBuilder.Options);
                _context.Database.EnsureCreated();
            });

            // Add services to the container.
            services.AddControllers();
            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Add DI for application layer
            services.AddApplication();

            // Add DI for infrastructure layer
            services.AddInfrastructure();

            // Map controller exception to Http status code
            services.AddProblemDetails(options =>
            {
                options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
                //options.MapToStatusCode<NotFoundEntityException>(StatusCodes.Status403Forbidden);
                //options.MapToStatusCode<NotFoundEntityException>(StatusCodes.Status404NotFound);
                options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
                options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
                options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            });

            services.AddControllers().AddProblemDetailsConventions();

            // Assign Actions to Documents by Convention
            services.AddMvc(options =>
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention())
            );

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            //services.AddControllers().AddApplicationPart(typeof(UserService).Assembly);
        }

        internal class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                var controllerNamespace = controller.ControllerType.Namespace;
                if (controllerNamespace != null)
                {
                    var apiVersion = controllerNamespace.Split('.').Last().ToLower();
                    controller.ApiExplorer.GroupName = apiVersion;
                }
            }
        }

        public static HttpClient GetClient()
        {
            // On réinitialise la base mémoire à chaque test.
            _keepAliveConnection.Close();
            _keepAliveConnection.Open();
            HttpClient client = HostTest.GetTestClient();

            return client;
        }

        public static APIContext GetContext()
        {
            return _context;
        }
    }
}
