using System.Collections.ObjectModel;
using System.Linq;
using SonyAudioControl.Model;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class SoundSettingViewModel : ViewModelBase
    {
        private string _title;
        private bool _isAvailable;
        private object _currentValue;
        private SoundSetting.Candidate _selectedOption;

        public SoundSettingViewModel(SoundSetting soundSetting)
        {
            Title = soundSetting.Title;
            IsAvailable = soundSetting.IsAvailable;
            Options = new ObservableCollection<SoundSetting.Candidate>(soundSetting.Candidates.Where(option => option.IsAvailable));
            SelectedOption = Options
                .SingleOrDefault(option => soundSetting.CurrentValue == option.Value);
            CurrentValue = soundSetting.CurrentValue;
            TargetType = soundSetting.Type;
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
            set => SetProperty(ref _currentValue, ConvertValue(value));
        }

        public SoundSetting.Candidate SelectedOption
        {
            get => _selectedOption;
            set => SetProperty(ref _selectedOption, value);
        }

        public ObservableCollection<SoundSetting.Candidate> Options { get; }

        public string TargetType { get; }

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
    }
}
