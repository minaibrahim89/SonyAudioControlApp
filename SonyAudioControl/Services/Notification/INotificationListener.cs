using System;
using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Notification
{
    public interface INotificationListener
    {
        Task SubscribeForNotificationsAsync(string deviceUrl, Action<DeviceNotification> onNotification);
    }
}
