using Geotab.Checkmate.ObjectModel;
using Geotab.Checkmate;
using Geotab.Checkmate.ObjectModel.Engine;
using GeoTabTest.Application.Interfaces;

namespace GeoTabTest.Application
{
    public class APIProviderService : IAPIProviderService
    {
        private readonly GeotabApiAuthenticationConfig _config;
        private readonly API _api;

        public APIProviderService(GeotabApiAuthenticationConfig config)
        {
            _config = config;
            _api = new API(_config.UserName, _config.Password, null, _config.Database, _config.ServerName);
        }

        public API GetAPI()
        {
            return _api;
        }

        public async Task<List<Device>?> GetDevicesAsync()
        {
            return await _api.CallAsync<List<Device>>("Get", typeof(Device));
        }

        public async Task AuthenticateAsync() {
            await _api.AuthenticateAsync();
        }

        public async Task<double?> GetOdometerInformation(Id deviceId)
        {
            // Search for status data based on the current device and the odometer reading
            var statusDataSearch = new StatusDataSearch
            {
                DeviceSearch = new DeviceSearch(deviceId),
                DiagnosticSearch = new DiagnosticSearch(KnownId.DiagnosticOdometerAdjustmentId),
                FromDate = DateTime.MaxValue
            };
            // Retrieve the odometer status data
            var statusData = await _api.CallAsync<IList<StatusData>>("Get", typeof(StatusData), new { search = statusDataSearch });
            if (statusData?.Count > 0)
                return statusData[0].Data ?? 0;
            return null;
        }

        public async Task<DeviceStatusInfo?> GetDevicesStatusInfoList(Id deviceId)
        {
            // Get the Device Status Info which contains the current latitude and longitude for this device
            var results = await _api.CallAsync<List<DeviceStatusInfo>>("Get", typeof(DeviceStatusInfo), new
            {
                search = new DeviceStatusInfoSearch
                {
                    DeviceSearch = new DeviceSearch
                    {
                        Id = deviceId
                    }
                }
            });
            if (results?.Count > 0)
                return results[0];
            else return null;
        }
    }
}