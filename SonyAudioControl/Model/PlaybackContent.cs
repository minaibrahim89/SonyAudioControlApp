namespace SonyAudioControl.Model
{
    public class PlaybackContent
    {
        public string Source { get; set; }
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public ContentMeta Content { get; set; }
        public int DurationMsec { get; set; }
        public int PositionMsec { get; set; }
        public bool IsRepeatOn => RepeatType == "on";
        public string RepeatType { get; set; }
        public string Title { get; set; }
        public StateInfo State { get; set; }
        public string Uri { get; set; }
    }

    public class ContentMeta
    {
        public string ThumbnailUrl { get; set; }
    }

    public class StateInfo
    {
        public string State { get; set; }
    }
}
