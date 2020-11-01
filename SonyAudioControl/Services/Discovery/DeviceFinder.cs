using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Rssdp;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Discovery
{
    public class DeviceFinder : IDeviceFinder
    {
        private readonly HttpClient _httpClient;

        public DeviceFinder()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<DeviceDescription>> SearchForUpnpDevicesAsync()
        {
            try
            {
                using (var deviceLocator = new SsdpDeviceLocator())
                {
                    var foundDevices = await deviceLocator.SearchAsync("urn:schemas-sony-com:service:ScalarWebAPI:1");

                    return await Task.WhenAll(foundDevices.Select(d => GetDeviceDescription(d.DescriptionLocation)).Where(x => x != null));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<DeviceDescription> GetDeviceDescription(Uri descriptionLocation)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(descriptionLocation))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        return null;

                    var xmlStream = await response.Content.ReadAsStreamAsync();
                    var description = (DeviceDescription)new XmlSerializer(typeof(DeviceDescription)).Deserialize(xmlStream);
                    description.DescriptionLocation = descriptionLocation;

                    return description;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
