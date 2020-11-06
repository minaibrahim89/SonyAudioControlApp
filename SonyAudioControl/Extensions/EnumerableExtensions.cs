using System.Collections;
using System.Linq;

namespace SonyAudioControl.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            return enumerable == null || !enumerable.Cast<object>().Any();
        }
    }
}
