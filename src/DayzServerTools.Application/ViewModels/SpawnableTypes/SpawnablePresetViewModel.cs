using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public class SpawnablePresetViewModel : ObservableFluentValidator<SpawnablePreset, SpawnablePresetValidator>,
    IImporter<IEnumerable<string>>
{
    private readonly IDialogFactory _dialogFactory;

    public string Preset
    {
        get => _model.Preset ?? "";
        set
        {
            SetProperty(_model.Preset, value, _model, (m, v) => _model.Preset = value, true);
            OnPropertyChanged(nameof(PresetSpecified));
            OnPropertyChanged(nameof(ItemsSpecified));
        }
    }
    public bool PresetSpecified => _model.PresetSpecified;
    public bool ItemsSpecified => _model.ItemsSpecified;
    public double Chance
    {
        get => _model.Chance;
        set => SetProperty(_model.Chance, value, _model, (m, v) => _model.Chance = value, true);
    }
    public SpawnableItem DefaultItem => _model.Items.FirstOrDefault();
    public ObservableCollection<SpawnableItem> Items => _model.Items;

    public RelayCommand ImportClassnamesCommand { get; }

    public SpawnablePresetViewModel(SpawnablePreset model, SpawnablePresetValidator validator)
        : base(model, validator)
    {
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
