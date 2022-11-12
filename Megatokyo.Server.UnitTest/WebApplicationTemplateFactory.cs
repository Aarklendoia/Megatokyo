using Megatokyo.Infrastructure.Repository.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Megatokyo.Server.UnitTest
{
    internal class WebApplicationTemplateFactory : WebApplicationFactory<Program>
    {
        public Action<APIContext>? InsertData { get; set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(serviceProvider =>
                {
                    var dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                      .UseInMemoryDatabase("Tests")
                      .UseApplicationServiceProvider(serviceProvider)
                      .Options;
                    APIContext context = new(dbContextOptions);
                    context.Database.EnsureDeleted();
                    InsertData?.Invoke(context);
                    return context;
                });
            });
            return base.CreateHost(builder);
        }
    }
}
