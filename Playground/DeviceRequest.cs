using System.Collections.Generic;
using Newtonsoft.Json;

namespace SonyAudioControl.Model
{
    public class DeviceRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 42;

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("params")]
        public object[] Params { get; set; }
    }
}
