using System.Collections.ObjectModel;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public class SpawnablePresetsCollectionProxy : IImporter<IEnumerable<string>>
{
    public string Name { get; }
    public ObservableCollection<SpawnablePresetViewModel> Presets { get; }

    public SpawnablePresetsCollectionProxy(string name, ObservableCollection<SpawnablePresetViewModel> presets)
    {
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
            return new SpawnablePresetViewModel(preset);
        });
        Presets.AddRange(presetVMs);
    }
}
