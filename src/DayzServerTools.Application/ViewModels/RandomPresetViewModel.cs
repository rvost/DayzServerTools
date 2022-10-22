using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public class RandomPresetViewModel : ObservableObject, IImporter<IEnumerable<string>>
{
    private RandomPreset _model;
    private readonly IDialogFactory _dialogFactory;

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

    public RelayCommand ImportClassnamesCommand { get; }

    public RandomPresetViewModel(RandomPreset model)
    {
        _model = model;
        _dialogFactory = Ioc.Default.GetRequiredService<IDialogFactory>();

        ImportClassnamesCommand = new RelayCommand(ImportClassnames);
    }

    public void Import(IEnumerable<string> classnames)
    {
        var total = classnames.Count();
        var items = classnames.Select(name =>
            new SpawnableItem(name, Math.Round(1.0 / total, 2))
        );
        Items.AddRange(items);
    }

    protected void ImportClassnames()
    {
        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new PresetImportStore(Items);
        dialog.ShowDialog();
    }
}