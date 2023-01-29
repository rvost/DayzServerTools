using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.Dialogs;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Trader.Validators;

namespace DayzServerTools.Application.ViewModels.Trader;

public partial class TraderCategoryViewModel : ObservableFluentValidator<TraderCategory, TraderCategoryValidator>,
    IImporter<IEnumerable<string>>, IImporter<IEnumerable<TraderItemViewModel>>
{
    private readonly IDialogFactory _dialogFactory;
    private readonly WorkspaceViewModel _workspace;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CopyItemsCommand), nameof(MoveItemsCommand),
        nameof(ProhibitBuyingCommand), nameof(ProhibitSellingCommand),
        nameof(SetBuyPriceCommand), nameof(SetSellPriceCommand),
        nameof(SetQuantityModifierCommand))]
    private IList selectedItems;

    public string Name
    {
        get => _model.CategoryName;
        set => SetProperty(_model.CategoryName, value, _model, (m, v) => m.CategoryName = v);
    }
    public ObservableCollection<TraderItemViewModel> Items { get; }

    public TraderCategoryViewModel(TraderCategory model, IDialogFactory dialogFactory, WorkspaceViewModel workspace) 
        : base(model, new())
    {
        _dialogFactory = dialogFactory;
        _workspace = workspace;

        Items = new(model.TraderItems.Select(m => new TraderItemViewModel(m)));

        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.TraderItems.Add(((TraderItemViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.TraderItems.Remove(((TraderItemViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    private bool CanExecuteBatchCommand() => SelectedItems is not null;

    [RelayCommand]
    private void ClassnamesImport()
    {
        var dialog = _dialogFactory.CreateClassnameImportDialog();
        dialog.Store = new TraderCategoryClassnameImportStore(this);
        dialog.ShowDialog();
    }

    [RelayCommand(CanExecute =nameof(CanExecuteBatchCommand))]
    private void CopyItems()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems
            .Cast<TraderItemViewModel>()
            .Select(item => new TraderItemViewModel(item.Model.Copy()));

        //TODO: Remove dependency on WorkspaceViewModel and TraderConfigViewModel
        var options = _workspace.Tabs
           .Where(t => t is TraderConfigViewModel)
           .ToList();

        var vm = new ExportViewModel<IEnumerable<TraderItemViewModel>>(items, options);

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.ShowDialog(vm);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void MoveItems()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>().ToList();

        //TODO: Remove dependency on WorkspaceViewModel and TraderConfigViewModel
        var options = _workspace.Tabs
           .Where(t => t is TraderConfigViewModel)
           .ToList();

        var vm = new ExportViewModel<IEnumerable<TraderItemViewModel>>(items, options);

        var dialog = _dialogFactory.CreateExportDialog();

        if (dialog.ShowDialog(vm) ?? false)
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void ProhibitBuying()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.ProhibitBuyingCommand.Execute(null));
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void ProhibitSelling()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.ProhibitSellingCommand.Execute(null));
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void SetBuyPrice(double price)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.BuyPrice = price);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void SetSellPrice(double price)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.SellPrice = price);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteBatchCommand))]
    private void SetQuantityModifier(string modifier)
    {
        if (SelectedItems is null || string.IsNullOrWhiteSpace(modifier))
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.Modifier = modifier);
    }

    public void Import(IEnumerable<string> classnames)
    {
        var items = classnames.Select(name => new TraderItemViewModel(new() { Name = name }));
        Items.AddRange(items);
    }

    public void Import(IEnumerable<TraderItemViewModel> items)
    {
        Items.AddRange(items);
    }
}
