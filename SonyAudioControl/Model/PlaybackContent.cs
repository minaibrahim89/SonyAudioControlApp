using SonyAudioControl.Extensions;

namespace SonyAudioControl.Model
{
    public class PlaybackContent
    {
        public string Source { get; set; }
        public string DisplayTitle => AlbumName.IsNullOrEmpty() ? Title : $"{Title} ({AlbumName})";
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public ContentMeta Content { get; set; }
        public int DurationMsec { get; set; }
        public int PositionMsec { get; set; }
        public bool IsRepeatOn => RepeatType == "on";
        public string RepeatType { get; set; }
        public string Title { get; set; }
        public StateInfo StateInfo { get; set; }
        public string Uri { get; set; }
    }

    public class ContentMeta
    {
        public string ThumbnailUrl { get; set; }
    }

    public class StateInfo
    {
        /// <summary>
        /// "PLAYING" - Content is being played
        /// "STOPPED" - Content is stopped
        /// "PAUSED" - Content is pausing
        /// "FORWARDING" - Content is being forwarded.
        /// </summary>
        public string State { get; set; }
    }
}
