using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.AvContent;
using SonyAudioControl.ViewModels.Base;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SonyAudioControl.ViewModels
{
    public class InputControlViewModel : ViewModelBase
    {
        private readonly IAvContentControl _avContentControl;
        private string _deviceUrl;
        private PlaybackContent _currentContent;
        private string _currentSourceName;
        private IconElement _playPauseIcon;
        private Timer _playbackUpdateTimer;
        private ImageSource _playbackThumbnail;
        private string _elapsedPlaybackTimeText;
        private string _playbackDurationText;
        private bool _showPlaybackProgress;

        public InputControlViewModel(IAvContentControl avContentControl)
        {
            _avContentControl = avContentControl;
            _playbackUpdateTimer = new Timer(UpdateCurrentContent, null, 0, 1000);

            SelectSourceCommand = new Command<string>(async source => await SelectSourceAsync(source));
            TogglePauseCommand = new Command(async () => await TogglePauseAsync());
            PlayNextCommand = new Command(async () => await PlayNextAsync());
            PlayPreviousCommand = new Command(async () => await PlayPreviousAsync());
        }

        private async void UpdateCurrentContent(object state)
        {
            if (CurrentContent?.StateInfo?.State != "PLAYING")
                return;

            _ = CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () => CurrentContent = await _avContentControl.GetPlayingContentInfoAsync(_deviceUrl));
        }

        public PlaybackSource[] SourceList { get; private set; }

        public PlaybackContent CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value, onChanged: OnCurrentContentChanged);
        }

        public bool ShowPlaybackProgress
        {
            get => _showPlaybackProgress;
            set => SetProperty(ref _showPlaybackProgress, value);
        }

        public string CurrentSourceName
        {
            get => _currentSourceName;
            set => SetProperty(ref _currentSourceName, value);
        }

        public ImageSource PlaybackThumbnail
        {
            get => _playbackThumbnail;
            set => SetProperty(ref _playbackThumbnail, value);
        }

        public string ElapsedPlaybackTimeText
        {
            get => _elapsedPlaybackTimeText;
            set => SetProperty(ref _elapsedPlaybackTimeText, value);
        }

        public string PlaybackDurationText
        {
            get => _playbackDurationText;
            set => SetProperty(ref _playbackDurationText, value);
        }

        public IconElement PlayPauseIcon
        {
            get => _playPauseIcon;
            set => SetProperty(ref _playPauseIcon, value);
        }

        public ICommand SelectSourceCommand { get; }
        public ICommand TogglePauseCommand { get; }
        public ICommand PlayNextCommand { get; }
        public ICommand PlayPreviousCommand { get; }

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
            var cast = _avContentControl.GetSourceListAsync(_deviceUrl, "cast");

            SourceList = (await Task.WhenAll(extInputs, storages, dlnas, cast)).SelectMany(source => source).ToArray();
        }

        private void OnCurrentContentChanged(PlaybackContent content)
        {
            CurrentSourceName = GetSourceName(content?.Source);
            UpdatePlaybackThumbnail(content);

            PlayPauseIcon = CurrentContent.StateInfo?.State switch
            {
                "PLAYING" => new SymbolIcon(Symbol.Pause),
                _ => new SymbolIcon(Symbol.Play)
            };

            ShowPlaybackProgress = content?.Source.StartsWith("netService:audio") == true;

            var elapsedTime = TimeSpan.FromMilliseconds(content?.PositionMsec ?? 0);
            var duration = TimeSpan.FromMilliseconds(content?.DurationMsec ?? 0);

            ElapsedPlaybackTimeText = (elapsedTime.Hours == 0 ? elapsedTime.Hours.ToString("0") : "") + $"{elapsedTime.Minutes:0}:{elapsedTime.Seconds:00}";
            PlaybackDurationText = (duration.Hours == 0 ? duration.Hours.ToString("0") : "") + $"{duration.Minutes:0}:{duration.Seconds:00}";
        }

        private void UpdatePlaybackThumbnail(PlaybackContent content)
        {
            var thumbnailUrl = content.Content?.ThumbnailUrl;

            if (thumbnailUrl == null && content.Source.StartsWith("alexa:audio") == true)
                thumbnailUrl = "ms-appx:///Assets/Images/Alexa.png";
            else if (thumbnailUrl == null && content.Source.StartsWith("cast:audio"))
                thumbnailUrl = "ms-appx:///Assets/Images/Chromecast.png";

            var thumbnailUri = new Uri(string.IsNullOrWhiteSpace(thumbnailUrl) ? "ms-appx:///Assets/Images/Music.png" : thumbnailUrl);

            if (PlaybackThumbnail is BitmapImage bitmap && bitmap.UriSource == thumbnailUri)
                return;

            PlaybackThumbnail = new BitmapImage(thumbnailUri);
        }

        public string GetSourceName(string source)
        {
            if (source.IsNullOrEmpty())
                return "";

            if (source.StartsWith("netService:audio"))
                return "Music Streaming Service";

            return source.StartsWith("alexa:audio") 
                ? "Alexa" 
                : SourceList.Single(s => s.Source == CurrentContent.Source).Title;
        }

        private async Task SelectSourceAsync(string source)
        {
            await _avContentControl.SetPlayContentAsync(_deviceUrl, source);
        }

        private async Task TogglePauseAsync()
        {
            await _avContentControl.TogglePausePlayingContentAsync(_deviceUrl);
        }

        private async Task PlayNextAsync()
        {
            await _avContentControl.SetPlayNextContentAsync(_deviceUrl);
        }

        private async Task PlayPreviousAsync()
        {
            await _avContentControl.SetPlayPreviousContentAsync(_deviceUrl);
        }

        internal void HandlePlayingContentInfoNotification(DeviceNotification notification)
        {
            var @params = (JObject)notification.Params[0];
            CurrentContent = @params.ToObject<PlaybackContent>();
        }
    }
}
