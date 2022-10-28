using System.Collections.ObjectModel;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores;

public class SpawnableTypePresetsImportStore : IClassnameImportStore
{
    private readonly ObservableCollection<SpawnablePresetViewModel> _target;

    public SpawnableTypePresetsImportStore(ObservableCollection<SpawnablePresetViewModel> target)
    {
        _target = target;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var items = classnames.Select(name => new SpawnableItem(name, 1));
        var presetVMs = items.Select(item =>
            {
                var preset = new SpawnablePreset() { Chance = 1 };
                preset.Items.Add(item);
                return new SpawnablePresetViewModel(preset);
            });
        _target.AddRange(presetVMs);
    }
}