using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Megatokyo.Domain.Exceptions;
using Megatokyo.Infrastructure;
using Megatokyo.Server.Models.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;
using TP.Logic;

IEnumerable<string> Versions = new[] { "1.0" };

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add DI for application builder.Services
builder.Services.AddHostedService<ConsumeScopedServiceHostedService>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

// Add DI for application layer
builder.Services.AddApplication();

// Add DI for infrastructure layer
builder.Services.AddInfrastructure();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Map controller exception to Http status code
builder.Services.AddProblemDetails(options =>
{
    options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<NotFoundEntityException>(StatusCodes.Status403Forbidden);
    options.MapToStatusCode<NotFoundEntityException>(StatusCodes.Status404NotFound);
    options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
    options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddControllers().AddProblemDetailsConventions();

// Configure Forwareded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
});

builder.Services.AddMvc(options =>
    options.Conventions.Add(new ApiExplorerGroupPerVersionConvention())
    );

// API Documentation discovery configuration
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    Versions.ToList()
      .ForEach(version =>
        config.SwaggerDoc($"{version}",
            new OpenApiInfo
            {
                Title = $"{Assembly.GetExecutingAssembly().GetName().Name}",
                Version = $"{version}",
                Description = "API for the Megatokyo application",
                Contact = new OpenApiContact
                {
                    Name = "Edouard Biton",
                    Url = new Uri("https://www.aarklendoia.com")
                }
            })
        );

    config.OperationFilter<RemoveVersionFromParameter>();
    config.DocumentFilter<ReplaceVersionWithExactValueInPath>();

    config.DocInclusionPredicate((version, description) =>
    {
        if (!description.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
        if (methodInfo != null && methodInfo.DeclaringType != null)
        {
            IEnumerable<ApiVersion> versions = methodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<ApiVersionAttribute>()
                .SelectMany(attribute => attribute.Versions);
            return versions.Any(v => $"{v}" == version);
        }
        else
            return false;
    });

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);
});

// Need for kubernetes health check API
builder.Services.AddHealthChecks()
        .AddCheck(
            name: "All probes",
            check: () => HealthCheckResult.Healthy(),
            tags: new[] { "start", "live", "ready" });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        Versions.ToList().ForEach(version => config.SwaggerEndpoint($"{version}/swagger.json", $"{Assembly.GetExecutingAssembly().GetName().Name} {version}"));
    });
}

app.UseProblemDetails();

app.UseForwardedHeaders();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination"));

app.Use((context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var pathBase))
    {
        context.Request.PathBase = new PathString(pathBase);
    }
    return next();
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Patch path base with forwarded path
app.Use(async (context, next) =>
{
    string? forwardedPath = context.Request.Headers["X-Forwarded-Path"].FirstOrDefault();
    if (!string.IsNullOrEmpty(forwardedPath))
    {
        context.Request.PathBase = forwardedPath;
    }

    await next().ConfigureAwait(false);
});

// Create directory for let's encrypt certification
Directory.CreateDirectory("./.well-known");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
    RequestPath = new PathString("/.well-known"),
    ServeUnknownFileTypes = true // serve extensionless file
});

app.Run();

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

internal class RemoveVersionFromParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Parameters.Any())
            return;

        var versionParameter = operation.Parameters
            .FirstOrDefault(p => p.Name.ToLower() == "version");

        if (versionParameter != null)
            operation.Parameters.Remove(versionParameter);
    }
}

internal class ReplaceVersionWithExactValueInPath : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc == null)
            throw new ArgumentNullException(nameof(swaggerDoc));

        var replacements = new OpenApiPaths();

        foreach (var (key, value) in swaggerDoc.Paths)
        {
            replacements.Add(key.Replace("{version}", swaggerDoc.Info.Version,
                    StringComparison.InvariantCulture), value);
        }

        swaggerDoc.Paths = replacements;
    }
}