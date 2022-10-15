using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public enum ClearTarget
{
    ValueFlags,
    UsageFlags,
    Tags
}

public partial class ItemTypesViewModel : ProjectFileViewModel<ItemTypes>, IDisposable
{

    [ObservableProperty]
    private ObservableCollection<ItemTypeViewModel> items = new();
    [ObservableProperty]
    private float quantityPercentage = 1;
    [ObservableProperty]
    private float lifetimePercentage = 1;
    [ObservableProperty]
    private float restockPercentage = 1;
    [ObservableProperty]
    private WorkspaceViewModel workspace = null;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AdjustLifetimeCommand), nameof(AdjustQuantityCommand),
        nameof(AdjustRestockCommand), nameof(SetCategoryCommand), nameof(ExportToNewFileCommand), 
        nameof(ExportToTraderCommand), nameof(AddValueFlagCommand), nameof(AddUsageFlagCommand), 
        nameof(AddTagCommand), nameof(ClearFlagsCommand))]
    private IList selectedItems;

    public IRelayCommand AddEmptyItemCommand { get; }
    public IRelayCommand<float?> AdjustQuantityCommand { get; }
    public IRelayCommand<float?> AdjustLifetimeCommand { get; }
    public IRelayCommand<float?> AdjustRestockCommand { get; }
    public IRelayCommand ExportToNewFileCommand { get; }
    public IRelayCommand ExportToTraderCommand { get; }
    public IRelayCommand ClassnamesImportCommand { get; }
    public IRelayCommand<VanillaFlag> SetCategoryCommand { get; }
    public IRelayCommand<UserDefinableFlag> AddValueFlagCommand { get; }
    public IRelayCommand<UserDefinableFlag> AddUsageFlagCommand { get; }
    public IRelayCommand<VanillaFlag> AddTagCommand { get; }
    public IRelayCommand<ClearTarget> ClearFlagsCommand { get; }
    public IRelayCommand ValidateCommand { get; }

    public ItemTypesViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        Model = new();
        FileName = "types.xml";

        AddEmptyItemCommand = new RelayCommand(AddEmptyItem);
        AdjustQuantityCommand = new RelayCommand<float?>(AdjustQuantity, (param)=> CanExecuteBatchCommand());
        AdjustLifetimeCommand = new RelayCommand<float?>(AdjustLifetime, (param) => CanExecuteBatchCommand());
        AdjustRestockCommand = new RelayCommand<float?>(AdjustRestock, (param) => CanExecuteBatchCommand());
        ExportToNewFileCommand = new RelayCommand<object>(ExportToNewFile, CanExecuteExportCommand);
        ExportToTraderCommand = new RelayCommand<object>(ExportToTrader, CanExecuteExportCommand);
        ClassnamesImportCommand = new RelayCommand(ClassnamesImport);
        SetCategoryCommand = new RelayCommand<VanillaFlag>(SetCategory, (param) => CanExecuteBatchCommand());
        AddValueFlagCommand = new RelayCommand<UserDefinableFlag>(AddValueFlag, (param) => CanExecuteBatchCommand());
        AddUsageFlagCommand = new RelayCommand<UserDefinableFlag>(AddUsageFlag, (param) => CanExecuteBatchCommand());
        AddTagCommand = new RelayCommand<VanillaFlag>(AddTag, (param) => CanExecuteBatchCommand());
        ClearFlagsCommand = new RelayCommand<ClearTarget>(ClearFlags, (param) => CanExecuteBatchCommand());
        ValidateCommand = new RelayCommand(Validate);

        Items.CollectionChanged += ItemsCollectionChanged;
    }

    public void CopyItemTypes(IEnumerable<ItemType> source)
    {
        Items.AddRange(
            source.Select(obj => new ItemTypeViewModel(obj.Copy(), Workspace))
        );
    }

    protected void AddEmptyItem()
        => Items.Add(new(new ItemType(), Workspace));
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
    protected void Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        Items.AsParallel().ForAll(item => item.ValidateSelf());

        var allErrors = Items.AsParallel()
            .Where(item => item.HasErrors)
            .Select(item =>
                {
                    var errorMessages = item.GetErrors().Select(r => r.ErrorMessage);
                    return new ValidationErrorInfo(this, item.Name, errorMessages);
                }
            );
        allErrors.ForAll(error => WeakReferenceMessenger.Default.Send(error));
    }
    protected void ExportToNewFile(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();

        var items = viewModels.Select(vm => vm.Model);
        Workspace.CreateItemTypes(items);
    }
    protected void ExportToTrader(object cmdParam)
    {
        var list = (IList)cmdParam;
        var viewModels = list.Cast<ItemTypeViewModel>();

        var items = viewModels.Select(vm => vm.Model);

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.Store = new ItemTypesToTraderExportStore(items);
        dialog.ShowDialog();
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

    protected override void OnLoad(Stream input, string filename)
    {
        var newItems = ItemTypes.ReadFromStream(input);
        Items.Clear();
        Items.AddRange(newItems.Types.Select(obj => new ItemTypeViewModel(obj, Workspace)));
    }
    protected override IFileDialog CreateOpenFileDialog()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        return dialog;
    }
    protected override bool CanSave()
    {
        var isEmpty = Model is null || Model.Types.Count == 0;
        var isValid = !Items.Any(i => i.HasErrors);
        return !isEmpty && isValid;
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
