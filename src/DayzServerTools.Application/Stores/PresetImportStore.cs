using System.Collections.ObjectModel;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores;

public class PresetImportStore : IClassnameImportStore
{
    private readonly ObservableCollection<SpawnableItem> _target;

    public PresetImportStore(ObservableCollection<SpawnableItem> target)
    {
        _target = target;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var total = classnames.Count();
        var items = classnames.Select(name =>
            new SpawnableItem(name, Math.Round(1.0 / total, 2))
        );
        _target.AddRange(items);
    }
}
