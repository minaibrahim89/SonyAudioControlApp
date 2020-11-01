using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Rssdp;
using SonyAudioControl.Services.Notification;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new NotificationListener().SubscribeForNotificationsAsync("http://192.168.0.247:10000/sony/");
                Console.ReadKey();
                using (var deviceLocator = new SsdpDeviceLocator())
                {
                    var foundDevices = deviceLocator.SearchAsync().Result;

                    Console.WriteLine(foundDevices.FirstOrDefault()?.DescriptionLocation.AbsoluteUri);
                    Console.ReadKey();

                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 60000);
            var multicastEndpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

            Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UdpSocket.Bind(LocalEndPoint);
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastEndpoint.Address, IPAddress.Any));
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

            Console.WriteLine("UDP-Socket setup done...\r\n");

            string SearchString = "M-SEARCH * HTTP/1.1\r\nHOST:239.255.255.250:1900\r\nMAN:\"ssdp:discover\"\r\nST:ssdp:all\r\nMX:3\r\n\r\n";

            UdpSocket.SendTo(Encoding.UTF8.GetBytes(SearchString), SocketFlags.None, multicastEndpoint);

            Console.WriteLine("M-Search sent...\r\n");

            byte[] ReceiveBuffer = new byte[64000];

            int ReceivedBytes = 0;

            while (true)
            {
                if (UdpSocket.Available > 0)
                {
                    ReceivedBytes = UdpSocket.Receive(ReceiveBuffer, SocketFlags.None);

                    if (ReceivedBytes > 0)
                    {
                        var response = Encoding.UTF8.GetString(ReceiveBuffer, 0, ReceivedBytes);
                        Console.WriteLine(response);
                    }
                }
            }
        }
    }
}
