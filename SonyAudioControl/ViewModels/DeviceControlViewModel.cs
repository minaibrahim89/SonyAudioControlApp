using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.AudioControl;
using SonyAudioControl.Services.Notification;
using SonyAudioControl.Services.SystemControl;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class DeviceControlViewModel : ViewModelBase
    {
        private readonly ISystemControl _systemControl;
        private readonly IAudioControl _audioControl;
        private readonly INotificationListener _notificationListener;
        private string _deviceUrl;
        private string _deviceName;
        private bool _isPowerOn;

        public DeviceControlViewModel(ISystemControl systemControl, IAudioControl audioControl, INotificationListener notificationListener)
        {
            _systemControl = systemControl;
            _audioControl = audioControl;
            _notificationListener = notificationListener;

            Volume = new VolumeViewModel(audioControl);

            TogglePowerCommand = new Command(async() => await TogglePowerAsync());
        }

        public string DeviceName
        {
            get => _deviceName;
            set => SetProperty(ref _deviceName, value);
        }

        public bool IsPowerOn
        {
            get => _isPowerOn; 
            set => SetProperty(ref _isPowerOn, value);
        }

        public VolumeViewModel Volume { get; }

        public ICommand TogglePowerCommand { get; }

        public override async Task InitializeAsync(object args)
        {
            var device = (DeviceViewModel)args;
            _deviceUrl = device.BaseUrl;
            DeviceName = $"{device.Name} ({device.ModelName})";
            await SetIsPowerOnAsync();
            await Volume.InitializeAsync(device);

            _ = _notificationListener.SubscribeForNotificationsAsync(_deviceUrl, HandleNotification);
        }

        private async Task SetIsPowerOnAsync()
        {
            var powerStatus = await _systemControl.GetPowerStatusAsync(_deviceUrl);

            IsPowerOn = powerStatus.Status == "active";
        }

        private void HandleNotification(DeviceNotification notification)
        {
            switch (notification.Method)
            {
                case "notifyPowerStatus": HandlePowerNotification(notification); break;
                case "notifyVolumeInformation": Volume.HandleVolumeNotification(notification); break;
            }
        }

        private void HandlePowerNotification(DeviceNotification notification)
        {
            IsPowerOn = notification.Params[0].status == "active";
        }

        private async Task TogglePowerAsync()
        {
            await _systemControl.SetPowerStatusAsync(_deviceUrl, !IsPowerOn);
        }
    }
}
