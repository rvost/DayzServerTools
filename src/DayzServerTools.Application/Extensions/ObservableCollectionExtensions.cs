using DayzServerTools.Library.Xml;
using System.Collections.ObjectModel;

namespace DayzServerTools.Application.Extensions;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
    {
        foreach(T item in range) collection.Add(item);
    }

    public static void RemoveAllEmpty(this ObservableCollection<VanillaFlag> flags)
    {
        var filtred = flags.Where(flag => string.IsNullOrEmpty(flag.Value)).ToList();
        foreach (var item in filtred)
        {
            flags.Remove(item);
        }
    }

    public static void RemoveAllEmpty(this ObservableCollection<UserDefinableFlag> flags)
    {
        var filtred = flags.Where(flag => string.IsNullOrEmpty(flag.Value)).ToList();
        foreach (var item in filtred)
        {
            flags.Remove(item);
        }
    }
}
