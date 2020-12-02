using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.Audio;
using SonyAudioControl.Services.AvContent;
using SonyAudioControl.Services.Notification;
using SonyAudioControl.Services.Storage;
using SonyAudioControl.Services.System;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class DeviceControlViewModel : ViewModelBase
    {
        private readonly ISystemControl _systemControl;
        private readonly INotificationListener _notificationListener;
        private readonly IStorage _storage;
        private string _deviceUrl;
        private string _deviceName;
        private bool _isPowerOn;

        public DeviceControlViewModel(ISystemControl systemControl, IAudioControl audioControl, IAvContentControl avContentControl,
            INotificationListener notificationListener, IStorage storage)
        {
            _systemControl = systemControl;
            _notificationListener = notificationListener;
            _storage = storage;
            Volume = new VolumeViewModel(audioControl);
            InputControl = new InputControlViewModel(avContentControl);

            TogglePowerCommand = new Command(async () => await TogglePowerAsync());
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

        public InputControlViewModel InputControl { get; }

        public ICommand TogglePowerCommand { get; }

        public override async Task InitializeAsync(object args)
        {
            var device = (DeviceViewModel)args;
            _deviceUrl = device.BaseUrl;

            try
            {
                DeviceName = device.Name == device.ModelName ? device.ModelName : $"{device.Name} ({device.ModelName})";
                await SetIsPowerOnAsync();
                await Volume.InitializeAsync(device);
                await InputControl.InitializeAsync(device);

                _ = _notificationListener.SubscribeForNotificationsAsync(_deviceUrl, HandleNotification);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex);
                await RemoveDeviceUrlFromStorageAsync(_deviceUrl);
                await Navigation.NavigateBackAsync();
            }
        }

        private async Task RemoveDeviceUrlFromStorageAsync(string deviceUrl)
        {
            if (!await _storage.TryGetAsync<IEnumerable<DeviceDescription>>("devices", out var devices))
                return;

            await _storage.SaveAsync("devices", devices.Where(d => d.Device.X_ScalarWebAPI_DeviceInfo.X_ScalarWebAPI_BaseURL != deviceUrl));
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
                case "notifyPlayingContentInfo": InputControl.HandlePlayingContentInfoNotification(notification); break;
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
