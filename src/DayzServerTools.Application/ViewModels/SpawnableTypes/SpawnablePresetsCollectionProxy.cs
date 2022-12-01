using System.Collections.ObjectModel;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public class SpawnablePresetsCollectionProxy : IImporter<IEnumerable<string>>
{
    private readonly PresetType _type;
    private readonly SpawnableTypesViewModelsFactory _viewModelsFactory;

    public string Name { get; }
    public ObservableCollection<SpawnablePresetViewModel> Presets { get; }

    public SpawnablePresetsCollectionProxy(PresetType type, string name, 
        ObservableCollection<SpawnablePresetViewModel> presets, SpawnableTypesViewModelsFactory viewModelsFactory)
    {
        _type = type;
        _viewModelsFactory = viewModelsFactory;
        Name = name;
        Presets = presets;
    }

    public void Import(IEnumerable<string> classnames)
    {
        var items = classnames.Select(name => new SpawnableItem(name, 1));
        var presetVMs = items.Select(item =>
        {
            var preset = new SpawnablePreset() { Chance = 1 };
            preset.Items.Add(item);
            
            return _viewModelsFactory.CreateSpawnablePresetViewModel(_type, preset);
        });
        Presets.AddRange(presetVMs);
    }
}
