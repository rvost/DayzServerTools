using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.RandomPresets;

public class RandomPresetViewModel : ObservableFluentValidator<RandomPreset, RandomPresetValidator>,
    IImporter<IEnumerable<string>>
{
    private readonly IDialogFactory _dialogFactory;

    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }
    public double Chance
    {
        get => _model.Chance;
        set => SetProperty(_model.Chance, value, _model, (m, v) => m.Chance = v, true);
    }
    public ObservableCollection<SpawnableItem> Items => _model.Items;

    public RelayCommand ImportClassnamesCommand { get; }

    public RandomPresetViewModel(RandomPreset model, IDialogFactory dialogFactory) : base(model, new())
    {
        _dialogFactory = dialogFactory;

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
