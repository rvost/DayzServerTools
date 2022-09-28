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

    public string Name
    {
        get => model.CategoryName;
        set => SetProperty(model.CategoryName, value, model, (m, v) => m.CategoryName = v);
    }
    public ObservableCollection<TraderItem> Items => model.TraderItems;

    public IRelayCommand<object> CopyItemsCommand { get; }
    public IRelayCommand<object> MoveItemsCommand { get; }

    public TraderCategoryViewModel(TraderCategory model)
    {
        _dialogFactory = Ioc.Default.GetService<IDialogFactory>();
        this.model = model;

        CopyItemsCommand = new RelayCommand<object>(CopyItems);
        MoveItemsCommand = new RelayCommand<object>(MoveItems);
    }

    public TraderCategoryViewModel() : this(new TraderCategory()) { }

    private void CopyItems(object cmdParam)
    {
        var list = (System.Collections.IList)cmdParam;
        var items = list.Cast<TraderItem>();

        var dialog = _dialogFactory.CreateExportDialog();
        dialog.Store = new TraderItemsExportStore(items, true);
        dialog.ShowDialog();
    }
    private void MoveItems(object cmdParam)
    {
        var list = (System.Collections.IList)cmdParam;
        var items = new List<TraderItem>(list.Cast<TraderItem>());

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
