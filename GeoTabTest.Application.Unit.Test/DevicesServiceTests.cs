using GeoTabTest.Application;
using GeoTabTest.Application.Interfaces;
using Geotab.Checkmate.ObjectModel;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeoTabTest.Application.Unit.Tests
{
    [TestFixture]
    public class DevicesServiceTests
    {
        private Mock<ILogger<DevicesService>> _loggerMock;
        private Mock<IAPIProviderService> _apiProviderServiceMock;
        private DevicesService _devicesService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DevicesService>>();
            _apiProviderServiceMock = new Mock<IAPIProviderService>();
            _devicesService = new DevicesService(_loggerMock.Object, _apiProviderServiceMock.Object);
        }

        [Test]
        public async Task WriteDevicesFilesAsync_Success()
        {
            var deviceId = Id.Create(new Guid());
            // Arrange
            var devices = new List<Device>
            {
                new Device { Id = deviceId, SerialNumber = "TestSerialNumber" }
            };

            var deviceStatusInfo = new DeviceStatusInfo { Latitude = 1.0, Longitude = 2.0 };
            var odometer = 100.0;

            _apiProviderServiceMock.Setup(apiProvider => apiProvider.AuthenticateAsync()).Returns(Task.CompletedTask);
            _apiProviderServiceMock.Setup(apiProvider => apiProvider.GetDevicesAsync()).ReturnsAsync(devices);
            _apiProviderServiceMock.Setup(apiProvider => apiProvider.GetDevicesStatusInfoList(It.IsAny<Id>())).ReturnsAsync(deviceStatusInfo);
            _apiProviderServiceMock.Setup(apiProvider => apiProvider.GetOdometerInformation(It.IsAny<Id>())).ReturnsAsync(odometer);

            // Act
            await _devicesService.WriteDevicesFilesAsync();

            // Assert
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.AuthenticateAsync(), Times.Once);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetDevicesAsync(), Times.Once);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetDevicesStatusInfoList(It.IsAny<Id>()), Times.Once);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetOdometerInformation(It.IsAny<Id>()), Times.Once);

            // Verify FileWriter
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CSVFiles", $"{deviceId}.csv");
            Assert.IsTrue(File.Exists(filePath));
            var fileContent = File.ReadAllText(filePath);
            Assert.IsTrue(fileContent.Contains($"{deviceId}"));
            Assert.IsTrue(fileContent.Contains(deviceStatusInfo.Latitude.ToString()));
            Assert.IsTrue(fileContent.Contains(deviceStatusInfo.Longitude.ToString()));
            Assert.IsTrue(fileContent.Contains(odometer.ToString()));
        }

        [Test]
        public async Task WriteDevicesFilesAsync_EmptyDevicesList()
        {
            // Arrange
            var devices = new List<Device>();

            _apiProviderServiceMock.Setup(apiProvider => apiProvider.AuthenticateAsync()).Returns(Task.CompletedTask);
            _apiProviderServiceMock.Setup(apiProvider => apiProvider.GetDevicesAsync()).ReturnsAsync(devices);

            // Act
            await _devicesService.WriteDevicesFilesAsync();

            // Assert
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.AuthenticateAsync(), Times.Once);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetDevicesAsync(), Times.Once);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetDevicesStatusInfoList(It.IsAny<Id>()), Times.Never);
            _apiProviderServiceMock.Verify(apiProvider => apiProvider.GetOdometerInformation(It.IsAny<Id>()), Times.Never);            

            // Verify FileWriter
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CSVFiles", "123.csv");
            Assert.IsFalse(File.Exists(filePath));
        }
    }
}
