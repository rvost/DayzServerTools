using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public class RandomPresetViewModel: ObservableObject
{
    private RandomPreset _model;

    public RandomPreset Model => _model;
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v);
    }
    public double Chance
    {
        get => _model.Chance;
        set => SetProperty(_model.Chance, value, _model, (m, v) => m.Chance = v);
    }
    public ObservableCollection<SpawnableItem> Items => _model.Items;

    public RandomPresetViewModel(RandomPreset model)
	{
		_model=model;
	}
}