﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public partial class SpawnableTypeViewModel : ObservableFluentValidator<SpawnableType, SpawnableTypeValidator>
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IRandomPresetsProvider _randomPresets;
    [ObservableProperty]
    private SpawnablePresetViewModel selectedPreset;

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
        set => SetProperty(_model.Damage.Min, value, _model, (m, v) => m.Damage.Min = v, true);
    }
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
    public ObservableCollection<SpawnablePresetViewModel> Cargo { get; } = new();
    public ObservableCollection<SpawnablePresetViewModel> Attachments { get; } = new();

    public IRelayCommand<PresetType> AddNewPresetCommand { get; }
    public IRelayCommand<PresetType> ImportClassnamesAsPresetsCommand { get; }

    public SpawnableTypeViewModel(SpawnableType model)
        : base(model, new(Ioc.Default.GetRequiredService<IRandomPresetsProvider>()))
    {
        _dialogFactory = Ioc.Default.GetRequiredService<IDialogFactory>();
        _randomPresets = Ioc.Default.GetRequiredService<IRandomPresetsProvider>();

        Cargo.AddRange(_model.Cargo.Select(preset =>
            new SpawnablePresetViewModel(preset, new(() => _randomPresets.AvailableCargoPresets))));
        Attachments.AddRange(_model.Attachments.Select(preset =>
            new SpawnablePresetViewModel(preset, new(() => _randomPresets.AvailableAttachmentsPresets))));

        Proxies = new List<SpawnablePresetsCollectionProxy>
        {
            new SpawnablePresetsCollectionProxy(PresetType.Cargo, "Cargo", Cargo),
            new SpawnablePresetsCollectionProxy(PresetType.Attachments, "Attachments", Attachments)
        };

        AddNewPresetCommand = new RelayCommand<PresetType>(AddNewPreset);
        ImportClassnamesAsPresetsCommand = new RelayCommand<PresetType>(ImportClassnamesAsPresets);

        Cargo.CollectionChanged += OnPresetsCollectionChanged;
        Attachments.CollectionChanged += OnPresetsCollectionChanged;
    }

    protected void AddNewPreset(PresetType type)
    {
        switch (type)
        {
            case PresetType.Cargo:
                Cargo.Add(
                    new SpawnablePresetViewModel(new SpawnablePreset(), new(() => _randomPresets.AvailableCargoPresets))
                );
                break;
            case PresetType.Attachments:
                Attachments.Add(
                    new SpawnablePresetViewModel(new SpawnablePreset(), new(() => _randomPresets.AvailableAttachmentsPresets))
                );
                break;
            default:
                break;
        }
    }
    protected void ImportClassnamesAsPresets(PresetType type)
    {
        var target = type == PresetType.Cargo ? Cargo : Attachments;

        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new SpawnableTypePresetsImportStore(type, target);
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