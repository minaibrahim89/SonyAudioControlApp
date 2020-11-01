using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Notification
{
    public class NotificationListener
    {
        public async Task SubscribeForNotificationsAsync(string deviceUrl)
        {
            var uri = new Uri(deviceUrl);
            SubscribeForNotificationsAsync(uri, "audio");
            //SubscribeForNotificationsAsync(uri, "system");
            //SubscribeForNotificationsAsync(uri, "avContent");
        }

        private async Task SubscribeForNotificationsAsync(Uri deviceUri, string lib)
        {
            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(new Uri($"ws://{deviceUri.Authority}/sony/{lib}"), CancellationToken.None);
                    var request = CreateRequest(1, Array.Empty<object>(), Array.Empty<object>());
                    var requestJson = JsonConvert.SerializeObject(request);
                    
                    await socket.SendAsync(Encoding.UTF8.GetBytes(requestJson), WebSocketMessageType.Text, true, CancellationToken.None);

                    await Receive(socket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.Message}");
                }
            }
        }

        private async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            do
            {
                WebSocketReceiveResult result;

                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var responseJson = await reader.ReadToEndAsync();
                        await OnNotificationReceivedAsync(socket, responseJson);
                    }
                }
            } 
            while (true);
        }

        private async Task OnNotificationReceivedAsync(ClientWebSocket socket, string message)
        {
            var response = JsonConvert.DeserializeObject<dynamic>(message);

            if (response.id == 1)
            {
                var enabled = (IEnumerable<dynamic>)response.result[0].enabled;
                var disabled = (IEnumerable<dynamic>)response.result[0].disabled;
                var enable = enabled.Concat(disabled);

                var request = CreateRequest(42, enable, null);
                var requestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore } );

                await socket.SendAsync(Encoding.UTF8.GetBytes(requestJson), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Console.WriteLine($"Message received: {message}");
            }
        }

        private object CreateRequest(int id, IEnumerable<object> enable, IEnumerable<object> disable)
        {
            return new DeviceRequest
            {
                Id = id,
                Method = "switchNotifications",
                Version = "1.0",
                Params = new[]
                {
                    new
                    {
                        enabled = enable?.ToArray(),
                        disabled = disable?.ToArray()
                    }
                }
            };
        }
    }
}
