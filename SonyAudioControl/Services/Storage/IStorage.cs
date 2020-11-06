using System.Threading.Tasks;

namespace SonyAudioControl.Services.Storage
{
    public interface IStorage
    {
        Task<bool> TryGetAsync<T>(string key, out T value);

        Task SaveAsync<T>(string key, T value);

        Task RemoveAsync(string key);
    }
}
