using Newtonsoft.Json;

namespace SonyAudioControl.Model
{
    public class SystemInformation
    {
        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("bdAddr")]
        public string BluetoothAddress { get; set; }

        [JsonProperty("deviceID")]
        public string DeviceId { get; set; }

        [JsonProperty("esn")]
        public string ESN { get; set; }

        [JsonProperty("generation")]
        public string Generation { get; set; }

        [JsonProperty("helpUrl")]
        public string HelpUrl { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("initialPowerOnTime")]
        public string InitialPowerOnTime { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("lastPowerOnTime")]
        public string LastPowerOnTime { get; set; }

        [JsonProperty("macAddr")]
        public string MACAddress { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("ssid")]
        public string SSID { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("wirelessMacAddr")]
        public string WirelessMACAddress { get; set; }
    }
}
