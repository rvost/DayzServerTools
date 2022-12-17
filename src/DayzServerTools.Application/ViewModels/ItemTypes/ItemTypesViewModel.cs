using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Application.ViewModels.RandomPresets;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Xml;
using ItemTypesModel = DayzServerTools.Library.Xml.ItemTypes;

namespace DayzServerTools.Application.ViewModels.ItemTypes;

public partial class ItemTypesViewModel : ProjectFileViewModel<ItemTypesModel>, IDisposable
{
    private readonly ItemTypesViewModelsFactory _viewModelFactory;
    private readonly WorkspaceViewModel _workspace;
    [ObservableProperty]
    private ObservableCollection<ItemTypeViewModel> items = new();
    [ObservableProperty]
    private float quantityPercentage = 1;
    [ObservableProperty]
    private float lifetimePercentage = 1;
    [ObservableProperty]
    private float restockPercentage = 1;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AdjustLifetimeCommand), nameof(AdjustQuantityCommand),
        nameof(AdjustRestockCommand), nameof(SetCategoryCommand), nameof(ExportToNewFileCommand),
        nameof(ExportToTraderCommand), nameof(AddValueFlagCommand), nameof(AddUsageFlagCommand),
        nameof(AddTagCommand), nameof(ClearFlagsCommand), nameof(ExportToSpawnableTypesCommand),
        nameof(ExportToRandomPresetsCommand))]
    private IList selectedItems;

    public IRelayCommand AddEmptyItemCommand { get; }
    public IRelayCommand<float?> AdjustQuantityCommand { get; }
    public IRelayCommand<float?> AdjustLifetimeCommand { get; }
    public IRelayCommand<float?> AdjustRestockCommand { get; }
    public IRelayCommand ExportToNewFileCommand { get; }
    public IRelayCommand ExportToSpawnableTypesCommand { get; }
    public IRelayCommand ExportToRandomPresetsCommand { get; }
    public IRelayCommand ExportToTraderCommand { get; }
    public IRelayCommand ClassnamesImportCommand { get; }
    public IRelayCommand<VanillaFlag> SetCategoryCommand { get; }
    public IRelayCommand<UserDefinableFlag> AddValueFlagCommand { get; }
    public IRelayCommand<UserDefinableFlag> AddUsageFlagCommand { get; }
    public IRelayCommand<VanillaFlag> AddTagCommand { get; }
    public IRelayCommand<ClearTarget> ClearFlagsCommand { get; }

    public ItemTypesViewModel(string fileName, ItemTypesModel model, IDialogFactory dialogFactory, IValidator<ItemTypesModel> validator, 
        WorkspaceViewModel workspace, ItemTypesViewModelsFactory viewModelFactory): base(fileName, model, validator, dialogFactory)
    {
        _viewModelFactory = viewModelFactory;
        _workspace = workspace;

        Items.AddRange(model.Types.Select(obj => _viewModelFactory.Create(obj)));

        AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
        AdjustQuantityCommand = new RelayCommand<float?>(AdjustQuantity, (param) => CanExecuteBatchCommand());
        AdjustLifetimeCommand = new RelayCommand<float?>(AdjustLifetime, (param) => CanExecuteBatchCommand());
        AdjustRestockCommand = new RelayCommand<float?>(AdjustRestock, (param) => CanExecuteBatchCommand());
        ExportToNewFileCommand = new RelayCommand<object>(ExportToNewFile, CanExecuteExportCommand);
        ExportToSpawnableTypesCommand = new RelayCommand<object>(ExportToSpawnableTypes, CanExecuteExportCommand);
        ExportToRandomPresetsCommand = new RelayCommand<object>(ExportToRandomPresets, CanExecuteExportCommand);
        ExportToTraderCommand = new RelayCommand<object>(ExportToTrader, CanExecuteExportCommand);
        ClassnamesImportCommand = new RelayCommand(ClassnamesImport);
        SetCategoryCommand = new RelayCommand<VanillaFlag>(SetCategory, (param) => CanExecuteBatchCommand());
        AddValueFlagCommand = new RelayCommand<UserDefinableFlag>(AddValueFlag, (param) => CanExecuteBatchCommand());
        AddUsageFlagCommand = new RelayCommand<UserDefinableFlag>(AddUsageFlag, (param) => CanExecuteBatchCommand());
        AddTagCommand = new RelayCommand<VanillaFlag>(AddTag, (param) => CanExecuteBatchCommand());
        ClearFlagsCommand = new RelayCommand<ClearTarget>(ClearFlags, (param) => CanExecuteBatchCommand());

        Items.CollectionChanged += ItemsCollectionChanged;
    }

    public void CopyItemTypes(IEnumerable<ItemType> source)
    {
        Items.AddRange(
            source.Select(obj => _viewModelFactory.Create(obj.Copy()))
        );
    }

    protected void AddEmptyItem()
        => Items.Add(_viewModelFactory.Create(new ItemType()));
    protected bool CanExecuteBatchCommand() => SelectedItems is not null;
    protected bool CanExecuteExportCommand(object param) => param is not null;
    protected void AdjustQuantity(float? factor)
    {
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        foreach (var item in viewModels)
        {
            item.AdjustQuantity(factor ?? QuantityPercentage);
        }
        QuantityPercentage = 1;
    }
    protected void AdjustLifetime(float? factor)
    {
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        foreach (var item in viewModels)
        {
            item.AdjustLifetime(factor ?? LifetimePercentage);
        }
        LifetimePercentage = 1;
    }
    protected void AdjustRestock(float? factor)
    {
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        foreach (var item in viewModels)
        {
            item.AdjustRestock(factor ?? RestockPercentage);
        }
        RestockPercentage = 1;
    }

    protected void ExportToNewFile(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();

        var items = viewModels.Select(vm => vm.Model);
        _workspace.CreateItemTypes(items);
    }
    protected void ExportToSpawnableTypes(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();
        var classnames = viewModels.Select(vm => vm.Name);

        var options = _workspace.Tabs
            .Where(t => t is SpawnableTypesViewModel)
            .ToList();
        var vm = new ExportViewModel<IEnumerable<string>>(classnames, options);
        var dialog = _dialogFactory.CreateExportDialog();
        dialog.ShowDialog(vm);
    }
    protected void ExportToRandomPresets(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();
        var classnames = viewModels.Select(vm => vm.Name);

        var options = _workspace.Tabs
            .Where(t => t is RandomPresetsViewModel)
            .ToList();
        var vm = new ExportViewModel<IEnumerable<string>>(classnames, options);
        var dialog = _dialogFactory.CreateExportDialog();
        dialog.ShowDialog(vm);
    }
    protected void ExportToTrader(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();
        var classnames = viewModels.Select(vm => vm.Name);

        var items = viewModels.Select(vm => vm.Model);
        
        var options = _workspace.Tabs
           .Where(t => t is TraderConfigViewModel)
           .ToList();
        var vm = new ExportViewModel<IEnumerable<string>>(classnames, options);
        
        var dialog = _dialogFactory.CreateExportDialog();
        dialog.ShowDialog(vm);
    }
    protected void ClassnamesImport()
    {
        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new ItemTypesClassnameImportStore(this);
        dialog.ShowDialog();
    }
    protected void SetCategory(VanillaFlag category)
    {
        if (category == null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        viewModels.AsParallel().ForAll(vm => vm.Category = category);
    }
    protected void AddValueFlag(UserDefinableFlag flag)
    {
        if (flag == null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        viewModels.AsParallel().ForAll(vm => vm.AddValueFlagCommand.Execute(flag));
    }
    protected void AddUsageFlag(UserDefinableFlag flag)
    {
        if (flag == null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        viewModels.AsParallel().ForAll(vm => vm.AddUsageFlagCommand.Execute(flag));
    }
    protected void AddTag(VanillaFlag flag)
    {
        if (flag == null)
        {
            return;
        }
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        viewModels.AsParallel().ForAll(vm => vm.AddTagCommand.Execute(flag));
    }
    protected void ClearFlags(ClearTarget target)
    {
        var viewModels = SelectedItems.Cast<ItemTypeViewModel>();
        viewModels.AsParallel().ForAll(vm => vm.ClearFlagsCommand.Execute(target));
    }

    protected override bool Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        var itemsErrors = Items.AsParallel()
            .Select(item => new { item.Name, Result = item.ValidateSelf() })
            .Where(x => !x.Result.IsValid)
            .Select(x => new ValidationErrorInfo(this, x.Name, x.Result.Errors.Select(x => x.ErrorMessage)))
            .ToList();

        bool itemsHaveErrors = itemsErrors.Any();

        if (itemsHaveErrors)
        {
            itemsErrors.AsParallel().ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        return res.IsValid && !itemsHaveErrors;
    }

    private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.Types.Add(((ItemTypeViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.Types.Remove(((ItemTypeViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        Items.CollectionChanged -= ItemsCollectionChanged;
    }
}
