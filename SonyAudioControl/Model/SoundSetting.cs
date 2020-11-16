using Newtonsoft.Json;

namespace SonyAudioControl.Model
{
    public class SoundSetting
    {
        [JsonProperty("candidate")]
        public Candidate[] Candidates { get; set; }

        [JsonProperty("currentValue")]
        public string CurrentValue { get; set; }

        [JsonProperty("deviceUIInfo")]
        public string DeviceUiInfo { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("titleTextID")]
        public string TitleTextId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public class Candidate
        {
            [JsonProperty("isAvailable")]
            public bool IsAvailable { get; set; }

            [JsonProperty("max")]
            public double Max { get; set; }

            [JsonProperty("min")]
            public double Min { get; set; }

            [JsonProperty("step")]
            public double Step { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("titleTextID")]
            public string TitleTextId { get; set; }

            public string Value { get; set; }
        }
    }
}
