using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

public enum PresetType
{
    Cargo,
    Attachments
}

public partial class SpawnableTypeViewModel : ObservableValidator
{
    private readonly SpawnableType _model;
    private readonly IDialogFactory _dialogFactory;

    [ObservableProperty]
    private SpawnablePresetViewModel selectedPreset;

    public SpawnableType Model => _model;
    [MinLength(1)]
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v);
    }
    public object Hoarder
    {
        get => _model.Hoarder;
        set => SetProperty(_model.Hoarder, value, _model, (m, v) => m.Hoarder = v);
    }
    [CustomValidation(typeof(SpawnableTypesValidation), nameof(SpawnableTypesValidation.ValidateChance))]
    public double MinDamage
    {
        get => _model.Damage.Min;
        set => SetProperty(_model.Damage.Min, value, _model, (m, v) => m.Damage.Min = v, true);
    }
    [CustomValidation(typeof(SpawnableTypesValidation), nameof(SpawnableTypesValidation.ValidateChance))]
    public double MaxDamage
    {
        get => _model.Damage.Max;
        set => SetProperty(_model.Damage.Max, value, _model, (m, v) => m.Damage.Max = v, true);
    }
    public string Tag
    {
        get => _model.Tag.Value;
        set => SetProperty(_model.Tag.Value, value, _model, (m, v) => m.Tag.Value = v);
    }
    public IEnumerable<SpawnablePresetsCollectionProxy> Proxies { get; }
    [CustomValidation(typeof(SpawnableTypeViewModel), nameof(ValidateSpawnablePresets))]
    public ObservableCollection<SpawnablePresetViewModel> Cargo { get; } = new();
    [CustomValidation(typeof(SpawnableTypeViewModel), nameof(ValidateSpawnablePresets))]
    public ObservableCollection<SpawnablePresetViewModel> Attachments { get; } = new();

    public IRelayCommand<PresetType> AddNewPresetCommand { get; }
    public IRelayCommand<PresetType> ImportClassnamesAsPresetsCommand { get; }

    public SpawnableTypeViewModel(SpawnableType model)
    {
        _model = model;
        _dialogFactory = Ioc.Default.GetRequiredService<IDialogFactory>();

        Cargo.AddRange(_model.Cargo.Select(preset => new SpawnablePresetViewModel(preset)));
        Attachments.AddRange(_model.Attachments.Select(preset => new SpawnablePresetViewModel(preset)));
        Proxies = new List<SpawnablePresetsCollectionProxy>
        {
            new SpawnablePresetsCollectionProxy( "Cargo",Cargo),
            new SpawnablePresetsCollectionProxy("Attachments", Attachments)
        };

        AddNewPresetCommand = new RelayCommand<PresetType>(AddNewPreset);
        ImportClassnamesAsPresetsCommand = new RelayCommand<PresetType>(ImportClassnamesAsPresets);

        Cargo.CollectionChanged += OnPresetsCollectionChanged;
        Attachments.CollectionChanged += OnPresetsCollectionChanged;
    }

    public void ValidateSelf() => ValidateAllProperties();
    public static ValidationResult ValidateSpawnablePresets(ICollection<SpawnablePresetViewModel> presets,
        ValidationContext context)
    {
        presets.AsParallel().ForAll(s => s.ValidateSelf());
        var presetsWithErrors = presets.Where(s => s.HasErrors);
        if (presetsWithErrors.Any())
        {
            return presetsWithErrors.First().GetErrors().First();
        }
        else
        {
            return ValidationResult.Success;
        }
    }
    protected void AddNewPreset(PresetType type)
    {
        var newPreset = new SpawnablePresetViewModel(new SpawnablePreset());

        switch (type)
        {
            case PresetType.Cargo:
                Cargo.Add(newPreset);
                break;
            case PresetType.Attachments:
                Attachments.Add(newPreset);
                break;
            default:
                break;
        }
    }
    protected void ImportClassnamesAsPresets(PresetType type)
    {
        var target = type == PresetType.Cargo ? Cargo : Attachments;

        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new SpawnableTypePresetsImportStore(target);
        dialog.ShowDialog();
    }

    private void OnPresetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ObservableCollection<SpawnablePreset> target =
           (sender == Cargo) ? Model.Cargo : Model.Attachments;


        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    target.Add(((SpawnablePresetViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    target.Remove(((SpawnablePresetViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }
}

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
