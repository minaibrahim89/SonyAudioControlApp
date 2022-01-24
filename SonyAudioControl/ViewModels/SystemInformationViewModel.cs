using SonyAudioControl.Extensions;
using SonyAudioControl.Model;
using SonyAudioControl.ViewModels.Base;
using System.Linq;

namespace SonyAudioControl.ViewModels
{
    public class SystemInformationViewModel : ViewModelBase
    {
        private string _text;

        public SystemInformationViewModel(SystemInformation systemInformation)
        {
            Text = string.Join("\n", systemInformation.ToDictionary().Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}
