using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Discovery
{
    public class DeviceFinder : IDeviceFinder, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Socket _multicastSocket;
        private readonly IPEndPoint _multicastEndpoint;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public DeviceFinder()
        {
            _httpClient = new HttpClient();
            _multicastSocket = CreateMulticastSocket(out _multicastEndpoint);
        }

        public async Task<IEnumerable<DeviceDescription>> SearchForUpnpDevicesAsync()
        {
            try
            {
                await _semaphore.WaitAsync();
                SendMulticastSearchRequest(_multicastSocket, _multicastEndpoint);
                var deviceUrls = await DiscoverDeviceLocationsAsync(_multicastSocket).ConfigureAwait(false);

                return await GetDeviceDescriptionsAsync(deviceUrls).ConfigureAwait(false);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private Socket CreateMulticastSocket(out IPEndPoint multicastEndpoint)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, 60000);
            multicastEndpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.Bind(localEndPoint);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastEndpoint.Address, IPAddress.Any));
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

            return udpSocket;
        }

        private static void SendMulticastSearchRequest(Socket multicastSocket, IPEndPoint endPoint)
        {
            var searchString = CreateSsdpDiscoveryMessage();
            multicastSocket.SendTo(Encoding.UTF8.GetBytes(searchString), SocketFlags.None, endPoint);
        }

        private static string CreateSsdpDiscoveryMessage()
        {
            return @"M-SEARCH * HTTP/1.1
HOST:239.255.255.250:1900
MAN: ""ssdp:discover""
ST:urn:schemas-sony-com:service:ScalarWebAPI:1
MX:10

";
        }

        private static async Task<List<Uri>> DiscoverDeviceLocationsAsync(Socket multicastSocket)
        {
            var receiveBuffer = new byte[64000];

            var deviceUrls = new List<Uri>();

            for (var retryCount = 0; retryCount < 10; ++retryCount)
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
                retryCount = 0;
            }

            return deviceUrls;
        }

        private async Task<IEnumerable<DeviceDescription>> GetDeviceDescriptionsAsync(List<Uri> deviceUrls)
        {
            return await Task.WhenAll(deviceUrls.Select(GetDeviceDescription).Where(x => x != null));
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

        public void Dispose()
        {
            // Optionally disconnect before disposal if needed.
            try
            {
                _multicastSocket.Disconnect(false);
            }
            catch
            {
                /* ignore */
            }
            _multicastSocket.Dispose();
            _httpClient.Dispose();
        }
    }
}
