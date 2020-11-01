using Newtonsoft.Json;

namespace SonyAudioControl.Model
{
    public class DeviceResponse<T> where T : class
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("result")]
        public T[] Result { get; set; }
    }
}
