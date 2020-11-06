using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.AudioControl;
using SonyAudioControl.Services.Notification;
using SonyAudioControl.ViewModels.Base;
using Windows.UI.Xaml.Controls;

namespace SonyAudioControl.ViewModels
{
    public class DeviceControlViewModel : ViewModelBase
    {
        private readonly IAudioControl _audioControl;
        private readonly INotificationListener _notificationListener;
        private string _deviceUrl;
        private string _deviceName;
        private IconElement _volumeIcon;
        private IconElement _muteIcon;
        private int _volume;
        private int _maxVolume;
        private int minVolume;
        private int _volumeStep;
        private bool _isMuted;

        public DeviceControlViewModel(IAudioControl audioControl, INotificationListener notificationListener)
        {
            _audioControl = audioControl;
            _notificationListener = notificationListener;
            _volumeIcon = new SymbolIcon(Symbol.Volume);
            _muteIcon = new SymbolIcon(Symbol.Volume);

            ToggleMuteCommand = new Command(ToggleMuteAsync);
        }

        public string DeviceName
        {
            get => _deviceName;
            set => SetProperty(ref _deviceName, value);
        }

        public int MaxVolume
        {
            get => _maxVolume;
            set => SetProperty(ref _maxVolume, value);
        }

        public int MinVolume
        {
            get => minVolume;
            set => SetProperty(ref minVolume, value);
        }

        public int VolumeStep
        {
            get => _volumeStep;
            set => SetProperty(ref _volumeStep, value);
        }
        
        public int Volume
        {
            get => _volume;
            set => SetProperty(ref _volume, value, onChanged: SetVolumeAsync);
        }

        public bool IsMuted
        {
            get => _isMuted;
            set => SetProperty(ref _isMuted, value, onChanged: isMuted => MuteIcon = new SymbolIcon(IsMuted ? Symbol.Mute : Symbol.Volume));
        }

        public IconElement MuteIcon
        {
            get => _muteIcon;
            set => SetProperty(ref _muteIcon, value, onChanged: _ => VolumeIcon = new SymbolIcon(IsMuted ? Symbol.Mute : Symbol.Volume));
        }

        public IconElement VolumeIcon
        {
            get => _volumeIcon;
            set => SetProperty(ref _volumeIcon, value);
        }

        public ICommand ToggleMuteCommand { get; set; }

        public override async Task InitializeAsync(object args)
        {
            var device = (DeviceViewModel)args;
            _deviceUrl = device.BaseUrl;
            DeviceName = $"{device.Name} ({device.ModelName})";
            var volumeInfo = await _audioControl.GetVolumeInfoAsync(_deviceUrl);
            MaxVolume = volumeInfo.MaxVolume;
            MinVolume = volumeInfo.MinVolume;
            VolumeStep = volumeInfo.Step;
            IsMuted = volumeInfo.IsMuted;
            Volume = volumeInfo.Volume;

            _ = _notificationListener.SubscribeForNotificationsAsync(_deviceUrl, HandleNotification);
        }

        private void HandleNotification(DeviceNotification notification)
        {
            if (notification.Method == "notifyVolumeInformation")
            {
                Volume = (int)notification.Params[0].volume;
                IsMuted = notification.Params[0].mute == "on";
            }
        }

        private void SetVolumeAsync(int volume)
        {
            _audioControl.SetVolumeAsync(_deviceUrl, volume);
        }

        private async void ToggleMuteAsync()
        {
            await _audioControl.SetMuteAsync(_deviceUrl, !IsMuted);
            
            IsMuted ^= true;
        }
    }
}
