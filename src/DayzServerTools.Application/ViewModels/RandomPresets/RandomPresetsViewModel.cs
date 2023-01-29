using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using RandomPresetsModel = DayzServerTools.Library.Xml.RandomPresets;

namespace DayzServerTools.Application.ViewModels.RandomPresets;

public partial class RandomPresetsViewModel : ProjectFileViewModel<RandomPresetsModel>, IDisposable
{
    [ObservableProperty]
    private RandomPresetViewModel selectedPreset;

    public IEnumerable<RandomPresetsCollectionProxy> Proxies { get; }
    public ObservableCollection<RandomPresetViewModel> CargoPresets { get; } = new();
    public ObservableCollection<RandomPresetViewModel> AttachmentsPresets { get; } = new();

    public RandomPresetsViewModel(string fileName, RandomPresetsModel model, IValidator<RandomPresetsModel> validator,
        IDialogFactory dialogFactory) : base(fileName, model, validator, dialogFactory)
    {
        CargoPresets.AddRange(
            model.CargoPresets.Select(preset => new RandomPresetViewModel(preset, _dialogFactory))
            );
        AttachmentsPresets.AddRange(
            model.AttachmentsPresets.Select(preset => new RandomPresetViewModel(preset, _dialogFactory))
            );

        Proxies = new List<RandomPresetsCollectionProxy>
        {
            new RandomPresetsCollectionProxy( "Cargo", CargoPresets),
            new RandomPresetsCollectionProxy("Attachments", AttachmentsPresets)
        };

        CargoPresets.CollectionChanged += OnPresetsCollectionChanged;
        AttachmentsPresets.CollectionChanged += OnPresetsCollectionChanged;
    }

    protected override bool Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        bool cargoHaveErrors = ReportPresetsErrors("Cargo", CargoPresets);
        bool attachmentsHaveErrors = ReportPresetsErrors("Attachments", AttachmentsPresets);

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        return res.IsValid && !cargoHaveErrors && !attachmentsHaveErrors;
    }

    [RelayCommand]
    private void NewCargoPreset()
        => CargoPresets.Add(new(new(), _dialogFactory));

    [RelayCommand]
    private void NewAttachmentsPreset()
        => AttachmentsPresets.Add(new(new(), _dialogFactory));

    private bool ReportPresetsErrors(string colName, ICollection<RandomPresetViewModel> presets)
    {
        var errorInfos = presets.AsParallel()
          .Select(preset => new { preset.Name, Result = preset.ValidateSelf() })
          .Where(x => !x.Result.IsValid)
          .Select(x => new ValidationErrorInfo(this, $"({colName}):{x.Name}", x.Result.Errors.Select(x => x.ErrorMessage)))
          .ToList();

        errorInfos.AsParallel().ForAll(error => WeakReferenceMessenger.Default.Send(error));

        return errorInfos.Any();
    }
    
    private void OnPresetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ObservableCollection<RandomPreset> target =
            (sender == CargoPresets) ? Model.CargoPresets : Model.AttachmentsPresets;


        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    target.Add(((RandomPresetViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    target.Remove(((RandomPresetViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        CargoPresets.CollectionChanged -= OnPresetsCollectionChanged;
        AttachmentsPresets.CollectionChanged -= OnPresetsCollectionChanged;
    }
}
