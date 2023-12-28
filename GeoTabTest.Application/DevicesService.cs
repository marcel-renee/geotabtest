using Geotab.Checkmate.ObjectModel;
using Geotab.Checkmate;
using GeoTabTest.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GeoTabTest.Application
{

    public class DevicesService : IDevicesService
    {
        private static readonly string _destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), "CSVFiles");

        private readonly ILogger _logger;
        private readonly IAPIProviderService _aPIProviderService;

        public DevicesService(

            ILogger<DevicesService> logger,
            IAPIProviderService aPIProviderService
            )
        {
            _logger = logger;
            _aPIProviderService = aPIProviderService;
        }

        public async Task WriteDevicesFilesAsync()
        {

            Directory.CreateDirectory(_destinationFolder);

            _logger.LogInformation("Authenticating Geotab API");
            await _aPIProviderService.AuthenticateAsync();

            // Get all devices
            _logger.LogInformation("Calling Get Devices - API");
            var devices = await _aPIProviderService.GetDevicesAsync();
            if (devices?.Count > 0)
            {
                var tasks = devices.Select(async device =>
                {
                    if (device.Id is null)
                        return;
                    _logger.LogInformation($"Calling Get Status Info - API - {device.Id}");
                    var deviceStatusInfo = await _aPIProviderService.GetDevicesStatusInfoList(device.Id);
                    _logger.LogInformation("Getting Odometer Info - API");
                    var odometer = await _aPIProviderService.GetOdometerInformation(device.Id);
                    if (deviceStatusInfo is not null && odometer is not null)
                    {
                        // Print the results to the console
                        //ID, timestamp, VIN, Coordinates, and Odometer.                    
                        var separator = "|";
                        var newLine = device.Id + separator
                                        + DateTime.Now.ToUniversalTime() + separator
                                        + device.SerialNumber + separator
                                        + deviceStatusInfo.Latitude + separator
                                        + deviceStatusInfo.Longitude + separator
                                        + odometer;

                        _logger.LogInformation("Writing To File");
                        FileWriter(device.Id, newLine);
                    }
                }).ToArray();
                Task.WaitAll(tasks);
            }
        }

        private static void FileWriter(Id id, string newLine)
        {
            using (var writer = new StreamWriter(Path.Combine(_destinationFolder, id + ".csv"), append: true))
            {
                writer.WriteLine(newLine);
            }
        }


    }
}