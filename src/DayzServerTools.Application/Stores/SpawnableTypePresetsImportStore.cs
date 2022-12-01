using System.Collections.ObjectModel;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores;

public class SpawnableTypePresetsImportStore : IClassnameImportStore
{
    private readonly ObservableCollection<SpawnablePresetViewModel> _target;
    private readonly PresetType _type;
    private readonly SpawnableTypesViewModelsFactory _viewModelsFactory;

    public SpawnableTypePresetsImportStore(PresetType type, ObservableCollection<SpawnablePresetViewModel> target,
        SpawnableTypesViewModelsFactory viewModelsFactory)
    {
        _target = target;
        _type = type;
        _viewModelsFactory = viewModelsFactory;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var items = classnames.Select(name => new SpawnableItem(name, 1));
        var presetVMs = items.Select(item =>
            {
                var preset = new SpawnablePreset() { Chance = 1 };
                preset.Items.Add(item);
                return _viewModelsFactory.CreateSpawnablePresetViewModel(_type, preset);
            });
        _target.AddRange(presetVMs);
    }
}
