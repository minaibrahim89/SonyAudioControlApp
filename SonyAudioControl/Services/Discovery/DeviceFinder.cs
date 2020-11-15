using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
            using var multicastSocket = CreateMulticastSocket(out var endPoint);
            const string searchString = @"M-SEARCH * HTTP/1.1
HOST:239.255.255.250:1900
MAN: ""ssdp:discover""
ST:urn:schemas-sony-com:service:ScalarWebAPI:1
MX:10

";
            multicastSocket.SendTo(Encoding.UTF8.GetBytes(searchString), SocketFlags.None, endPoint);

            var receiveBuffer = new byte[64000];

            var deviceUrls = new List<Uri>();

            for (var i = 0; i < 10; ++i)
            {
                if (multicastSocket.Available == 0)
                {
                    await Task.Delay(1000);
                    continue;
                }

                var receivedBytes = await multicastSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);
                var deviceUrl = response.Split(Environment.NewLine).Single(line => line.StartsWith("LOCATION")).Substring(9).Trim();
                deviceUrls.Add(new Uri(deviceUrl));
                i = 0;
            }

            return await Task.WhenAll(deviceUrls.Select(GetDeviceDescription).Where(x => x != null));
        }

        private Socket CreateMulticastSocket(out IPEndPoint multicastEndpoint)
        {
            var LocalEndPoint = new IPEndPoint(IPAddress.Any, 60000);
            multicastEndpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.Bind(LocalEndPoint);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastEndpoint.Address, IPAddress.Any));
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

            return udpSocket;
        }

        private async Task<DeviceDescription> GetDeviceDescription(Uri descriptionLocation)
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
    }
}
