using MediatR;

namespace Megatokyo.Server.Models.Services
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }

    public class ScopedProcessingService(IMediator mediator, ILogger<ScopedProcessingService> logger) : IScopedProcessingService
    {
        private readonly ILogger _logger = logger;

        public async Task DoWork(CancellationToken stoppingToken)
        {
            WebSiteParser webSiteParser = new(mediator);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Website parser running.");
                await webSiteParser.ParseAsync();
                await Task.Delay(1000 * 60 * 5, stoppingToken);
            }
        }
    }
}
