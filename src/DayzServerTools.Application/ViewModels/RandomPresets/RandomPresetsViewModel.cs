using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;
using RandomPresetsModel = DayzServerTools.Library.Xml.RandomPresets;

namespace DayzServerTools.Application.ViewModels.RandomPresets;

public partial class RandomPresetsViewModel : ProjectFileViewModel<RandomPresetsModel>, IDisposable
{
    private readonly RandomPresetsValidator _validator;
    [ObservableProperty]
    private RandomPresetViewModel selectedPreset;

    public IEnumerable<RandomPresetsCollectionProxy> Proxies { get; }
    public ObservableCollection<RandomPresetViewModel> CargoPresets { get; } = new();
    public ObservableCollection<RandomPresetViewModel> AttachmentsPresets { get; } = new();

    public IRelayCommand NewCargoPresetCommand { get; }
    public IRelayCommand NewAttachmentsPresetCommand { get; }
    public IRelayCommand ValidateCommand { get; }

    public RandomPresetsViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        _validator = new();
        Model = new();
        FileName = "cfgrandompresets.xml";

        Proxies = new List<RandomPresetsCollectionProxy>
        {
            new RandomPresetsCollectionProxy( "Cargo", CargoPresets),
            new RandomPresetsCollectionProxy("Attachments", AttachmentsPresets)
        };

        NewCargoPresetCommand = new RelayCommand(() => CargoPresets.Add(new(new())));
        NewAttachmentsPresetCommand = new RelayCommand(() => AttachmentsPresets.Add(new(new())));
        ValidateCommand = new RelayCommand(Validate);

        CargoPresets.CollectionChanged += OnPresetsCollectionChanged;
        AttachmentsPresets.CollectionChanged += OnPresetsCollectionChanged;
    }

    public void Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        ReportPresetsErrors("Cargo", CargoPresets);
        ReportPresetsErrors("Attachments", AttachmentsPresets);

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }
    }
    protected override void OnLoad(Stream input, string filename)
    {
        var randomPresets = RandomPresetsModel.ReadFromStream(input);
        CargoPresets.AddRange(
            randomPresets.CargoPresets.Select(preset => new RandomPresetViewModel(preset))
            );
        AttachmentsPresets.AddRange(
            randomPresets.AttachmentsPresets.Select(preset => new RandomPresetViewModel(preset))
            );
    }
    protected override bool CanSave()
    {
        var isEmpty = CargoPresets.Count == 0 && AttachmentsPresets.Count == 0;
        var hasErrors = CargoPresets.Any(f => f.HasErrors) || AttachmentsPresets.Any(f => f.HasErrors);
        return !isEmpty && !hasErrors;
    }
    protected override IFileDialog CreateOpenFileDialog()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfgrandompresets*";
        return dialog;
    }

    private void ReportPresetsErrors(string colName, ICollection<RandomPresetViewModel> presets)
    {
        presets.AsParallel()
          .Select(preset => new { preset.Name, Result = preset.ValidateSelf() })
          .Where(x => !x.Result.IsValid)
          .Select(x => new ValidationErrorInfo(this, $"({colName}):{x.Name}", x.Result.Errors.Select(x => x.ErrorMessage)))
          .ForAll(error => WeakReferenceMessenger.Default.Send(error));
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
