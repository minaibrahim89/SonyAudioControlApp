using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SonyAudioControl.Extensions;
using SonyAudioControl.Services.Audio;
using SonyAudioControl.Services.Storage;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class SoundSettingsViewModel : ViewModelBase
    {
        private readonly IAudioControl _audioControl;
        private readonly IStorage _storage;
        private string _deviceUrl;

        public SoundSettingsViewModel(IAudioControl audioControl, IStorage storage)
        {
            _audioControl = audioControl;
            _storage = storage;
        }

        public ObservableCollection<SoundSettingViewModel> SoundSettings { get; }
            = new ObservableCollection<SoundSettingViewModel>();

        public override async Task InitializeAsync(object args)
        {
            await _storage.TryGetAsync(StorageKeys.CurrentDevice, out _deviceUrl);

            var soundSettings = await _audioControl.GetSoundSettingsAsync(_deviceUrl);
            SoundSettings.AddRange(soundSettings.Select(setting => new SoundSettingViewModel(setting)));
        }
    }
}
