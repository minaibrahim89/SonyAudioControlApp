namespace SonyAudioControl.Model
{    
    public class PlaybackSource
    {
        public string IconUrl { get; set; }
        public bool IsBrowsable { get; set; }
        public bool IsPlayable { get; set; }
        public string Meta { get; set; }
        public string PlayAction { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
    }
}
