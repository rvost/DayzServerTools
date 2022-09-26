using System.Collections.ObjectModel;

namespace DayzServerTools.Application.Extensions;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
    {
        foreach(T item in range) collection.Add(item);
    }
}
