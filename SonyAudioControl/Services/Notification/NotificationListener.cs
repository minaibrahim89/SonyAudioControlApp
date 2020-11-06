using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Notification
{
    public class NotificationListener : INotificationListener
    {
        public async Task SubscribeForNotificationsAsync(string deviceUrl, Action<DeviceNotification> onNotification)
        {
            var uri = new Uri(deviceUrl);
            await Task.WhenAll(
                SubscribeForNotificationsAsync(uri, "audio", onNotification),
                SubscribeForNotificationsAsync(uri, "system", onNotification),
                SubscribeForNotificationsAsync(uri, "avContent", onNotification));
        }

        private async Task SubscribeForNotificationsAsync(Uri deviceUri, string lib, Action<DeviceNotification> onNotification)
        {
            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(new Uri($"ws://{deviceUri.Authority}/sony/{lib}"), CancellationToken.None);
                    var request = CreateRequest(1, Array.Empty<object>(), Array.Empty<object>()).ToJson();
                    
                    await socket.SendAsync(Encoding.UTF8.GetBytes(request), WebSocketMessageType.Text, true, CancellationToken.None);

                    await Receive(socket, onNotification);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR - {ex.Message}");
                }
            }
        }

        private async Task Receive(ClientWebSocket socket, Action<DeviceNotification> onNotification)
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
                        await OnNotificationReceivedAsync(socket, responseJson, onNotification);
                    }
                }
            } while (true);
        }

        private async Task OnNotificationReceivedAsync(ClientWebSocket socket, string message, Action<DeviceNotification> onNotification)
        {
            var response = message.DeJson<DeviceResponse<dynamic>>();

            if (response.Id == 1)
            {
                var enabled = (IEnumerable<dynamic>)response.Result[0].enabled;
                var disabled = (IEnumerable<dynamic>)response.Result[0].disabled;
                var enable = enabled.Concat(disabled);

                var request = CreateRequest(42, enable, null).ToJson();
                await socket.SendAsync(Encoding.UTF8.GetBytes(request), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (response.Id == default)
            {
                Debug.WriteLine($"Message received: {message}");

                var notification = message.DeJson<DeviceNotification>();
                onNotification(notification);
            }
        }

        private DeviceRequest CreateRequest(int id, IEnumerable<object> enable, IEnumerable<object> disable)
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
