using MediatR;

namespace Megatokyo.Server.Models.Services
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }

    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ScopedProcessingService(IMediator mediator, ILogger<ScopedProcessingService> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            WebSiteParser webSiteParser = new(_mediator);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Website parser running.");
                await webSiteParser.ParseAsync();
                await Task.Delay(1000 * 60 * 5, stoppingToken);
            }
        }
    }
}
