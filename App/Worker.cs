using App.Helpers;

namespace App
{
    public class Worker : BackgroundService
    {
        private readonly Proxy _proxy;
        private readonly ILogger<Worker> _logger;

        private static DateTimeOffset Now => DateTimeOffset.Now;

        private static readonly TimeSpan Delay = TimeSpan.FromMinutes(1);

        public Worker(Proxy proxy, ILogger<Worker> logger)
        {
            _proxy = proxy;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            using var activity = OpenTelemetrySource.Instance.StartActivity();

            _logger.LogTrace("Starting worker at {time}", Now);

            await base.StartAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Delay, stoppingToken);

                using var activity = OpenTelemetrySource.Instance.StartActivity();

                _logger.LogTrace("Running worker at {time}", Now);

                await _proxy.PrintAsync();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            using var activity = OpenTelemetrySource.Instance.StartActivity();

            _logger.LogTrace("Stopping worker at {time}", Now);

            await base.StopAsync(stoppingToken);
        }
    }
}