using Newtonsoft.Json;

namespace SonyAudioControl.Model
{
    public class DeviceNotification
    {
        [JsonProperty("method")]
        public string Method { get; private set; }

        [JsonProperty("version")]
        public string Version { get; private set; }

        [JsonProperty("params")]
        public dynamic[] Params { get; private set; }
    }
}
