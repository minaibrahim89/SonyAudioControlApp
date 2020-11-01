using System.Collections.Generic;
using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Discovery
{
    public interface IDeviceFinder
    {
        Task<IEnumerable<DeviceDescription>> SearchForUpnpDevicesAsync();
    }
}
