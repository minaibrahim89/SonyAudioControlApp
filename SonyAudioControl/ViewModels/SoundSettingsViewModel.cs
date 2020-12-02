using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Audio;
using SonyAudioControl.Services.Notification;
using SonyAudioControl.Services.Storage;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class SoundSettingsViewModel : ViewModelBase
    {
        private readonly IAudioControl _audioControl;
        private readonly IStorage _storage;
        private readonly INotificationListener _notificationListener;
        private string _deviceUrl;

        public SoundSettingsViewModel(IAudioControl audioControl, IStorage storage, INotificationListener notificationListener)
        {
            _audioControl = audioControl;
            _storage = storage;
            _notificationListener = notificationListener;
        }

        public ObservableCollection<SoundSettingViewModel> SoundSettings { get; }
            = new ObservableCollection<SoundSettingViewModel>();

        public override async Task InitializeAsync(object args)
        {
            await _storage.TryGetAsync(StorageKeys.CurrentDevice, out _deviceUrl);

            var soundSettings = await _audioControl.GetSoundSettingsAsync(_deviceUrl);
            SoundSettings.AddRange(soundSettings.Select(setting => new SoundSettingViewModel(setting, _audioControl, _deviceUrl)));

            _ = _notificationListener.SubscribeForNotificationsAsync(_deviceUrl, HandleNotification);
        }

        private void HandleNotification(DeviceNotification notification)
        {
            switch (notification.Method)
            {
                case "notifySettingsUpdate": HandleSettingsUpdateNotification(notification); break;
            }
        }

        private void HandleSettingsUpdateNotification(DeviceNotification notification)
        {
            var apiMappingUpdate = notification.Params[0].apiMappingUpdate;

            var (target, currentValue) = ((string)apiMappingUpdate.target, (string)apiMappingUpdate.currentValue);
            var targetSetting = SoundSettings.SingleOrDefault(setting => setting.Target.Contains(target, StringComparison.OrdinalIgnoreCase));

            targetSetting.SetValue(currentValue);
        }
    }
}
