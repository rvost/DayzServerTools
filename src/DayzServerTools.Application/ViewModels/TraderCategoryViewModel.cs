using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.Stores;
using DayzServerTools.Library.Trader;


namespace DayzServerTools.Application.ViewModels;

public partial class TraderCategoryViewModel : ObservableObject
{
    private readonly IDialogFactory _dialogFactory;
    [ObservableProperty]
    private TraderCategory model;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CopyItemsCommand), nameof(MoveItemsCommand),
        nameof(ProhibitBuyingCommand), nameof(ProhibitSellingCommand),
        nameof(SetBuyPriceCommand), nameof(SetSellPriceCommand),
        nameof(SetQuantityModifierCommand))]
    private IList selectedItems;

    public string Name
    {
        get => model.CategoryName;
        set => SetProperty(model.CategoryName, value, model, (m, v) => m.CategoryName = v);
    }
    public ObservableCollection<TraderItemViewModel> Items { get; }

    public IRelayCommand CopyItemsCommand { get; }
    public IRelayCommand MoveItemsCommand { get; }
    public IRelayCommand ProhibitSellingCommand { get; }
    public IRelayCommand ProhibitBuyingCommand { get; }
    public IRelayCommand<double> SetBuyPriceCommand { get; }
    public IRelayCommand<double> SetSellPriceCommand { get; }
    public IRelayCommand<string> SetQuantityModifierCommand { get; }

    public TraderCategoryViewModel(TraderCategory model)
    {
        _dialogFactory = Ioc.Default.GetService<IDialogFactory>();
        this.model = model;
        Items = new(model.TraderItems.Select(m => new TraderItemViewModel(m)));

        CopyItemsCommand = new RelayCommand(CopyItems, () => CanExecuteBatchCommand());
        MoveItemsCommand = new RelayCommand(MoveItems, () => CanExecuteBatchCommand());
        ProhibitBuyingCommand = new RelayCommand(ProhibitBuying, () => CanExecuteBatchCommand());
        ProhibitSellingCommand = new RelayCommand(ProhibitSelling, () => CanExecuteBatchCommand());
        SetBuyPriceCommand = new RelayCommand<double>(SetBuyPrice, (param) => CanExecuteBatchCommand());
        SetSellPriceCommand = new RelayCommand<double>(SetSellPrice, (param) => CanExecuteBatchCommand());
        SetQuantityModifierCommand = new RelayCommand<string>(SetQuantityModifier, (param) => CanExecuteBatchCommand());

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

    public TraderCategoryViewModel() : this(new TraderCategory()) { }

    protected bool CanExecuteBatchCommand() => SelectedItems is not null;
    protected void CopyItems()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.Store = new TraderItemsExportStore(items, true);
        dialog.ShowDialog();
    }
    protected void MoveItems()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>().ToList();

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.Store = new TraderItemsExportStore(items, false);

        if (dialog.ShowDialog() ?? false)
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }
    }
    protected void ProhibitBuying()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.ProhibitBuyingCommand.Execute(null));
    }
    protected void ProhibitSelling()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.ProhibitSellingCommand.Execute(null));
    }
    protected void SetBuyPrice(double price)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.BuyPrice = price);
    }
    protected void SetSellPrice(double price)
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.SellPrice = price);
    }
    protected void SetQuantityModifier(string modifier)
    {
        if (SelectedItems is null || string.IsNullOrWhiteSpace(modifier))
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItemViewModel>();

        items.AsParallel().ForAll(item => item.Modifier = modifier);
    }
}
