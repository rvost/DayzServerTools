using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validation;

namespace DayzServerTools.Application.ViewModels;

public class SpawnablePresetViewModel : ObservableValidator, IImporter<IEnumerable<string>>
{
    private readonly SpawnablePreset _model;
    private readonly IDialogFactory _dialogFactory;

    public SpawnablePreset Model => _model;

    [CustomValidation(typeof(SpawnablePresetViewModel), nameof(ValidatePresetName))]
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
    [CustomValidation(typeof(SpawnChanceValidation), nameof(SpawnChanceValidation.ValidateChance))]
    public double Chance
    {
        get => _model.Chance;
        set => SetProperty(_model.Chance, value, _model, (m, v) => _model.Chance = value, true);
    }
    public SpawnableItem DefaultItem => _model.Items.FirstOrDefault();
    public ObservableCollection<SpawnableItem> Items => _model.Items;

    public RelayCommand ImportClassnamesCommand { get; }

    public SpawnablePresetViewModel(SpawnablePreset model)
        : base(new Dictionary<object, object>() { { "workspace", Ioc.Default.GetRequiredService<WorkspaceViewModel>() } })
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
    public void ValidateSelf() => ValidateAllProperties();
    public static ValidationResult ValidatePresetName(string name, ValidationContext context)
    {
        var workspace = (WorkspaceViewModel)context.Items["workspace"];
        if (!string.IsNullOrEmpty(name) && workspace.RandomPresetsLoaded)
        {
            if(workspace.AvailableCargoPresets.Contains(name) || 
                workspace.AvailableAttachmentsPresets.Contains(name))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Preset name '{name}' does not exist in context");
            }
        }
        return ValidationResult.Success;
    }
    protected void ImportClassnames()
    {
        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new PresetImportStore(Items);
        dialog.ShowDialog();
    }
}
