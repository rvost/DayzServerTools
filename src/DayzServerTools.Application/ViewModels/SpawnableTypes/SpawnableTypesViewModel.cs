﻿using System.Collections;
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
using FluentValidation;
using SpawnableTypesModel = DayzServerTools.Library.Xml.SpawnableTypes;

namespace DayzServerTools.Application.ViewModels.SpawnableTypes;

public partial class SpawnableTypesViewModel : ProjectFileViewModel<SpawnableTypesModel>,
    IImporter<IEnumerable<string>>, IDisposable
{
    private readonly SpawnableTypesValidator _validator;
    [ObservableProperty]
    private WorkspaceViewModel workspace;
    [ObservableProperty]
    private SpawnableTypeViewModel selectedItem;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportToNewFileCommand),
        nameof(SetMinDamageCommand), nameof(SetMaxDamageCommand))]
    private IList selectedItems;
    [ObservableProperty]
    private bool showTagColumn = false;
    public ObservableCollection<SpawnableTypeViewModel> Spawnables { get; set; } = new();

    public IRelayCommand AddSpawnableTypeCommand { get; }
    public IRelayCommand<object> ExportToNewFileCommand { get; }
    public IRelayCommand<double> SetMinDamageCommand { get; }
    public IRelayCommand<double> SetMaxDamageCommand { get; }
    public IRelayCommand ValidateCommand { get; }

    public SpawnableTypesViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        _validator = new();
        Model = new();
        FileName = "cfgspawnabletypes.xml";

        AddSpawnableTypeCommand = new RelayCommand(() => Spawnables.Add(new(new())));
        ExportToNewFileCommand = new RelayCommand<object>(ExportToNewFile, CanExecuteExportCommand);
        SetMinDamageCommand = new RelayCommand<double>(SetMinDamage, (param) => CanExecuteBatchCommand());
        SetMaxDamageCommand = new RelayCommand<double>(SetMaxDamage, (param) => CanExecuteBatchCommand());
        ValidateCommand = new RelayCommand(Validate);

        Spawnables.CollectionChanged += OnSpawnablesCollectionChanged;
    }

    public void CopySpawnableTypes(IEnumerable<SpawnableType> source)
    {
        Spawnables.AddRange(
            source.Select(obj => new SpawnableTypeViewModel(obj.Copy()))
        );
    }
    public void Import(IEnumerable<string> classnames)
    {
        var models = classnames.Select(name => new SpawnableType() { Name = name });
        var viewModels = models.Select(models => new SpawnableTypeViewModel(models));
        Spawnables.AddRange(viewModels);
    }
    public void Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        Spawnables.AsParallel()
            .Select(item => new { item.Name, Result = item.ValidateSelf() })
            .Where(x => !x.Result.IsValid)
            .Select(x => new ValidationErrorInfo(this, x.Name, x.Result.Errors.Select(x => x.ErrorMessage)))
            .ForAll(error => WeakReferenceMessenger.Default.Send(error));

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }
    }

    protected bool CanExecuteBatchCommand() => SelectedItems is not null;
    protected bool CanExecuteExportCommand(object cmdParam)
        => cmdParam is not null || SelectedItems is not null;
    protected void ExportToNewFile(object cmdParam)
    {
        var list = (IList)cmdParam ?? SelectedItems;
        var viewModels = list.Cast<SpawnableTypeViewModel>();

        var items = viewModels.Select(vm => vm.Model);
        Workspace.CreateSpawnableTypes(items);
    }
    protected void SetMinDamage(double value)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<SpawnableTypeViewModel>();
        viewModels.AsParallel().ForAll(spawnable => spawnable.MinDamage = value);
    }
    protected void SetMaxDamage(double value)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<SpawnableTypeViewModel>();
        viewModels.AsParallel().ForAll(spawnable => spawnable.MaxDamage = value);
    }

    protected override bool CanSave()
    {
        var isEmpty = Spawnables.Count == 0;
        var isValid = !Spawnables.Any(i => i.HasErrors);
        return !isEmpty && isValid;
    }
    protected override IFileDialog CreateOpenFileDialog()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        return dialog;
    }
    protected override void OnLoad(Stream input, string filename)
    {
        var spawnableTypes = SpawnableTypesModel.ReadFromStream(input);
        Spawnables.AddRange(
            spawnableTypes.Spawnables.Select(type => new SpawnableTypeViewModel(type))
            );
    }

    private void OnSpawnablesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.Spawnables.Add(((SpawnableTypeViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.Spawnables.Remove(((SpawnableTypeViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        Spawnables.CollectionChanged -= OnSpawnablesCollectionChanged;
    }
}
