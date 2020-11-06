using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.AudioControl;
using SonyAudioControl.ViewModels.Base;
using Windows.UI.Xaml.Controls;

namespace SonyAudioControl.ViewModels
{
    public class VolumeViewModel : ViewModelBase
    {
        private readonly IAudioControl _audioControl;
        private IconElement _volumeIcon;
        private IconElement _muteIcon;
        private string _deviceUrl;
        private int _value;
        private int _maxValue;
        private int minVolume;
        private int _stepSize;
        private bool _isMuted;

        public VolumeViewModel(IAudioControl audioControl)
        {
            _audioControl = audioControl;
            _volumeIcon = new SymbolIcon(Symbol.Volume);
            _muteIcon = new SymbolIcon(Symbol.Volume);

            ToggleMuteCommand = new Command(ToggleMuteAsync);
        }

        public int MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        public int MinValue
        {
            get => minVolume;
            set => SetProperty(ref minVolume, value);
        }

        public int StepSize
        {
            get => _stepSize;
            set => SetProperty(ref _stepSize, value);
        }

        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value, onChanged: SetVolumeAsync);
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
            
            var volumeInfo = await _audioControl.GetVolumeInfoAsync(_deviceUrl);
            MaxValue = volumeInfo.MaxVolume;
            MinValue = volumeInfo.MinVolume;
            StepSize = volumeInfo.Step;
            IsMuted = volumeInfo.IsMuted;
            Value = volumeInfo.Volume;
        }

        internal void HandleVolumeNotification(Model.DeviceNotification notification)
        {
            Value = (int)notification.Params[0].volume;
            IsMuted = notification.Params[0].mute == "on";
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
