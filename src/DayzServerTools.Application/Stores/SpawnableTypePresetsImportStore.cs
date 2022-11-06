using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.DependencyInjection;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.Stores;

public class SpawnableTypePresetsImportStore : IClassnameImportStore
{
    private readonly ObservableCollection<SpawnablePresetViewModel> _target;
    private readonly PresetType _type;
    public SpawnableTypePresetsImportStore(PresetType type, ObservableCollection<SpawnablePresetViewModel> target)
    {
        _target = target;
        _type = type;
    }

    public void Accept(IEnumerable<string> classnames)
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
        _target.AddRange(presetVMs);
    }
}