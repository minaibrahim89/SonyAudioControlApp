using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.AvContent;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class InputControlViewModel : ViewModelBase
    {
        private readonly IAvContentControl _avContentControl;
        private string _deviceUrl;
        private PlaybackContent _currentContent;
        private string _currentSourceName;

        public InputControlViewModel(IAvContentControl avContentControl)
        {
            _avContentControl = avContentControl;

            SelectSourceCommand = new Command<string>(async source => await SelectSourceAsync(source));
        }

        public PlaybackSource[] SourceList { get; private set; }

        public PlaybackContent CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value, onChanged: content => CurrentSourceName = GetSourceName(content?.Source));
        }

        public string CurrentSourceName
        {
            get => _currentSourceName;
            set => SetProperty(ref _currentSourceName, value);
        }

        public ICommand SelectSourceCommand { get; }

        public override async Task InitializeAsync(object args)
        {
            var device = (DeviceViewModel)args;
            _deviceUrl = device.BaseUrl;

            await LoadSourceListAsync();
            
            CurrentContent = await _avContentControl.GetPlayingContentInfoAsync(_deviceUrl);
        }

        private async Task LoadSourceListAsync()
        {
            var extInputs = _avContentControl.GetSourceListAsync(_deviceUrl, "extInput");
            var storages = _avContentControl.GetSourceListAsync(_deviceUrl, "storage");
            var dlnas = _avContentControl.GetSourceListAsync(_deviceUrl, "dlna");

            SourceList = (await Task.WhenAll(extInputs, storages, dlnas)).SelectMany(source => source).ToArray();
        }

        public string GetSourceName(string source)
        {
            if (source.IsNullOrEmpty())
                return "";

            if (source .StartsWith( "netService:audio"))
                return "Music Streaming Service";
            
            return SourceList.Single(s => s.Source == CurrentContent.Source).Title;
        }


        private async Task SelectSourceAsync(string source)
        {
            await _avContentControl.SetPlayContentAsync(_deviceUrl, source);
        }

        internal void HandlePlayingContentInfoNotification(DeviceNotification notification)
        {
            var @params = (JObject)notification.Params[0];
            CurrentContent = @params.ToObject<PlaybackContent>();
        }
    }
}
