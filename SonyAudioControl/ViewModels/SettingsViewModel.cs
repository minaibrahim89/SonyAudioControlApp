using SonyAudioControl.Services.Storage;
using SonyAudioControl.Services.System;
using SonyAudioControl.ViewModels.Base;
using System.Threading.Tasks;

namespace SonyAudioControl.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISystemControl _systemControl;
        private readonly IStorage _storage;
        private SystemInformationViewModel _systemInfo;
        private string _deviceUrl;

        public SettingsViewModel(ISystemControl systemControl, IStorage storage)
        {
            _systemControl = systemControl;
            _storage = storage;
        }

        public SystemInformationViewModel SystemInfo
        {
            get => _systemInfo;
            set => SetProperty(ref _systemInfo, value);
        }

        public override async Task InitializeAsync(object args)
        {
            await _storage.TryGetAsync(StorageKeys.CurrentDevice, out _deviceUrl);

            var systemInfo = await _systemControl.GetSystemInformationAsync(_deviceUrl);
            SystemInfo = new SystemInformationViewModel(systemInfo);
        }
    }
}
