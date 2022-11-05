using System.Collections.ObjectModel;

namespace DayzServerTools.Application.ViewModels.RandomPresets;

public class RandomPresetsCollectionProxy
{
    public string Name { get; }
    public ObservableCollection<RandomPresetViewModel> Presets { get; }

    public RandomPresetsCollectionProxy(string name, ObservableCollection<RandomPresetViewModel> presets)
    {
        Name = name;
        Presets = presets;
    }
}