using System.Collections;
using System.Collections.ObjectModel;

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
    [NotifyCanExecuteChangedFor(nameof(CopyItemsCommand), nameof(MoveItemsCommand))]
    private IList selectedItems;

    public string Name
    {
        get => model.CategoryName;
        set => SetProperty(model.CategoryName, value, model, (m, v) => m.CategoryName = v);
    }
    public ObservableCollection<TraderItem> Items => model.TraderItems;

    public IRelayCommand CopyItemsCommand { get; }
    public IRelayCommand MoveItemsCommand { get; }

    public TraderCategoryViewModel(TraderCategory model)
    {
        _dialogFactory = Ioc.Default.GetService<IDialogFactory>();
        this.model = model;

        CopyItemsCommand = new RelayCommand(CopyItems, () => CanExecuteBatchCommand());
        MoveItemsCommand = new RelayCommand(MoveItems, () => CanExecuteBatchCommand());
    }

    public TraderCategoryViewModel() : this(new TraderCategory()) { }

    protected bool CanExecuteBatchCommand() => SelectedItems is not null;
    protected void CopyItems()
    {
        if (SelectedItems is null)
        {
            return;
        }
        var items = SelectedItems.Cast<TraderItem>();

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
        var items = SelectedItems.Cast<TraderItem>();

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.Store = new TraderItemsExportStore(items, true);

        if (dialog.ShowDialog() ?? false)
        {
            foreach (var item in items)
            {
                model.TraderItems.Remove(item);
            }
        }
    }
}
