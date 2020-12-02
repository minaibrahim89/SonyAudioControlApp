using System.Collections.ObjectModel;
using System.Linq;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Audio;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class SoundSettingViewModel : ViewModelBase
    {
        private readonly IAudioControl _audioControl;
        private readonly string _deviceUrl;
        private string _title;
        private bool _isAvailable;
        private object _currentValue;
        private SoundSetting.Candidate _selectedOption;
        private bool _isInitialized;

        public SoundSettingViewModel(SoundSetting soundSetting, IAudioControl audioControl, string deviceUrl)
        {
            _audioControl = audioControl;
            _deviceUrl = deviceUrl;

            Title = soundSetting.Title;
            IsAvailable = soundSetting.IsAvailable;
            Options = new ObservableCollection<SoundSetting.Candidate>(soundSetting.Candidates.Where(c => soundSetting.Type != "enumTarget" || c.IsAvailable));
            SelectedOption = Options
                .SingleOrDefault(option => soundSetting.CurrentValue == option.Value);
            CurrentValue = ConvertValue(soundSetting.CurrentValue);
            TargetType = soundSetting.Type;
            Target = soundSetting.Target;

            _isInitialized = true;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public object CurrentValue
        {
            get => _currentValue;
            set
            {
                if (SetProperty(ref _currentValue, value) && _isInitialized)
                    _audioControl.SetSoundSettingAsync(_deviceUrl, Target, value?.ToString() ?? "");
            }
        }

        public SoundSetting.Candidate SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (SetProperty(ref _selectedOption, value) && _isInitialized)
                    _audioControl.SetSoundSettingAsync(_deviceUrl, Target, value?.Value ?? "");
            }
        }

        public ObservableCollection<SoundSetting.Candidate> Options { get; }

        public string TargetType { get; }

        public string Target { get; }

        public object ConvertValue(object value)
        {
            var str = (string)value;

            return TargetType switch
            {
                "booleanTarget" => bool.Parse(str),
                "integerTarget" => int.Parse(str),
                "doubleTarget" => double.Parse(str),
                _ => str
            };
        }

        public void SetValue(string value)
        {
            _currentValue = value;
            OnPropertyChanged(nameof(CurrentValue));

            if (TargetType == "enumTarget" || TargetType == "booleanTarget")
            {
                _selectedOption = Options.Single(opt => opt.Value == value);
                OnPropertyChanged(nameof(SelectedOption));
            }
        }
    }
}
