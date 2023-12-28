using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTabTest.Application.Interfaces
{
    public interface IDevicesService
    {
        Task WriteDevicesFilesAsync();
    }
}
