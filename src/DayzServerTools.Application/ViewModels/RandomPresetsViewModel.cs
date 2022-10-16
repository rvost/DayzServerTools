using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public partial class RandomPresetsViewModel : ProjectFileViewModel<RandomPresets>, IDisposable
{
    public ObservableCollection<RandomPresetViewModel> CargoPresets { get; } = new();
    public ObservableCollection<RandomPresetViewModel> AttachmentsPresets { get; } = new();

    public IRelayCommand NewCargoPresetCommand { get; }
    public IRelayCommand NewAttachmentsPresetCommand { get; }

    public RandomPresetsViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        Model = new();
        FileName = "cfgrandompresets.xml";

        NewCargoPresetCommand = new RelayCommand(() => CargoPresets.Add(new(new())));
        NewAttachmentsPresetCommand = new RelayCommand(() => AttachmentsPresets.Add(new(new())));

        CargoPresets.CollectionChanged += OnPresetsCollectionChanged;
        AttachmentsPresets.CollectionChanged += OnPresetsCollectionChanged;
    }

    protected override void OnLoad(Stream input, string filename)
    {
        var randomPresets = RandomPresets.ReadFromStream(input);
        CargoPresets.AddRange(
            randomPresets.CargoPresets.Select(preset => new RandomPresetViewModel(preset))
            );
        AttachmentsPresets.AddRange(
            randomPresets.AttachmentsPresets.Select(preset => new RandomPresetViewModel(preset))
            );
    }
    protected override bool CanSave() => true;
    protected override IFileDialog CreateOpenFileDialog()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfgrandompresets*";
        return dialog;
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
