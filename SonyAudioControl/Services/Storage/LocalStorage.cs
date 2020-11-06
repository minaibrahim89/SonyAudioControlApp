using System.Threading.Tasks;
using SonyAudioControl.Extensions;
using Windows.Storage;

namespace SonyAudioControl.Services.Storage
{
    public class LocalStorage : IStorage
    {
        private ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public Task<bool> TryGetAsync<T>(string key, out T value)
        {
            value = default;

            if (!LocalSettings.Values.TryGetValue(key, out var obj))
                return Task.FromResult(false);

            if (obj is string json)
                value = json.DeJson<T>();

            return Task.FromResult(true);
        }

        public Task SaveAsync<T>(string key, T value)
        {
            LocalSettings.Values[key] = value.ToJson();

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            LocalSettings.Values.Remove(key);

            return Task.CompletedTask;
        }
    }
}
