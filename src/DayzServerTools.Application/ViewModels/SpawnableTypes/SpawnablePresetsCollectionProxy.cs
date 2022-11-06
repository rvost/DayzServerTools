using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public class SpawnablePresetsCollectionProxy : IImporter<IEnumerable<string>>
{
    private readonly PresetType _type;
    public string Name { get; }
    public ObservableCollection<SpawnablePresetViewModel> Presets { get; }

    public SpawnablePresetsCollectionProxy(PresetType type, string name, ObservableCollection<SpawnablePresetViewModel> presets)
    {
        _type = type;
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
            var provider = Ioc.Default.GetService<IRandomPresetsProvider>();
            SpawnablePresetValidator validator = _type switch
            {
                PresetType.Attachments => new SpawnablePresetValidator(() => provider.AvailableAttachmentsPresets),
                PresetType.Cargo => new SpawnablePresetValidator(() => provider.AvailableCargoPresets),
                _ => null
            };
            return new SpawnablePresetViewModel(preset, validator);
        });
        Presets.AddRange(presetVMs);
    }
}
