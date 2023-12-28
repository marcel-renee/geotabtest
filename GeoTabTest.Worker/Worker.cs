using GeoTabTest.Application.Interfaces;

namespace GeoTabTest.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDevicesService _devicesService;

        public Worker(ILogger<Worker> logger, IDevicesService devicesService)
        {
            _logger = logger;
            _devicesService = devicesService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _devicesService.WriteDevicesFilesAsync();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }            
        }
    }
}