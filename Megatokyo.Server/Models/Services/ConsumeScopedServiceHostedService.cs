namespace Megatokyo.Server.Models.Services
{
    internal class ConsumeScopedServiceHostedService(IServiceProvider services,
        ILogger<ConsumeScopedServiceHostedService> logger) : BackgroundService
    {
        public IServiceProvider Services { get; } = services;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using var scope = Services.CreateScope();
            var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }
    }
}
