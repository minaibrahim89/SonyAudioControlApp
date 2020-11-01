namespace SonyAudioControl.Model
{
    public class VolumeInfo
    {
        public int MaxVolume { get; set; }
        public int MinVolume { get; set; }
        public bool IsMuted => Mute == "on";
        public string Mute { get; set; }
        public int Step { get; set; }
        public int Volume { get; set; }
    }
}
