using Newtonsoft.Json;

namespace SonyAudioControl.Extensions
{
    public static class StringExtensions
    {
        public static T DeJson<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
