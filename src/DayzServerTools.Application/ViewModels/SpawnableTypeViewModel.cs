using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public partial class SpawnableTypeViewModel : ObservableObject
{
    private readonly SpawnableType _model;
    private readonly WorkspaceViewModel _workspace;

    public SpawnableType Model => _model;
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
    public double MinDamage
    {
        get => _model.Damage.Min;
        set => SetProperty(_model.Damage.Min, value, _model, (m, v) => m.Damage.Min = v);
    }
    public double MaxDamage
    {
        get => _model.Damage.Max;
        set => SetProperty(_model.Damage.Max, value, _model, (m, v) => m.Damage.Max = v);
    }
    public string Tag
    {
        get => _model.Tag.Value;
        set => SetProperty(_model.Tag.Value, value, _model, (m, v) => m.Tag.Value = v);
    }
    public ObservableCollection<SpawnablePresetViewModel> Cargo { get; } = new();
    public ObservableCollection<SpawnablePresetViewModel> Attachments { get; } = new();
    public IEnumerable<string> AvailableCargoPresets => _workspace.AvailableCargoPresets;
    public IEnumerable<string> AvailableAttachmentsPresets => _workspace.AvailableAttachmentsPresets;

    public SpawnableTypeViewModel(SpawnableType model)
    {
        _model = model;
        _workspace = Ioc.Default.GetRequiredService<WorkspaceViewModel>();

        Cargo.AddRange(_model.Cargo.Select(preset => new SpawnablePresetViewModel(preset)));
        Attachments.AddRange(_model.Attachments.Select(preset => new SpawnablePresetViewModel(preset)));

        Cargo.CollectionChanged += OnPresetsCollectionChanged;
        Attachments.CollectionChanged += OnPresetsCollectionChanged;
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
