using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SonyAudioControl.Extensions
{
    internal static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }
}
