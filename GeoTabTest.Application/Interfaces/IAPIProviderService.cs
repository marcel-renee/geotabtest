using Geotab.Checkmate;
using Geotab.Checkmate.ObjectModel;

namespace GeoTabTest.Application.Interfaces
{
    public interface IAPIProviderService
    {
        Task AuthenticateAsync();
        API GetAPI();
        Task<List<Device>?> GetDevicesAsync();
        Task<DeviceStatusInfo?> GetDevicesStatusInfoList(Id deviceId);
        Task<double?> GetOdometerInformation(Id deviceId);
    }
}