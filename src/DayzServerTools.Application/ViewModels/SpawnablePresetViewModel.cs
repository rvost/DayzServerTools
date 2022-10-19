using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public class SpawnablePresetViewModel:ObservableObject
{
    private readonly SpawnablePreset _model;

    public SpawnablePreset Model => _model;
    public string Preset
    {
        get => _model.Preset ?? "";
        set
        {
            SetProperty(_model.Preset, value, _model, (m, v) => _model.Preset = value);
            OnPropertyChanged(nameof(PresetSpecified));
            OnPropertyChanged(nameof(ItemsSpecified));
        }
    }
    public bool PresetSpecified => _model.PresetSpecified;
    public bool ItemsSpecified => _model.ItemsSpecified;
    public double Chance
    {
        get=> _model.Chance;
        set => SetProperty(_model.Chance, value, _model, (m, v) => _model.Chance = value);
    }
    public SpawnableItem DefaultItem => _model.Items.FirstOrDefault();
    public ObservableCollection<SpawnableItem> Items => _model.Items;
    
    public SpawnablePresetViewModel(SpawnablePreset model)
    {
        _model = model;
    }
}
