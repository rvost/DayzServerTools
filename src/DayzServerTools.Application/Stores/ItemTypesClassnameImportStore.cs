using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores;

public interface IClassnameImportStore
{
    void Accept(IEnumerable<string> classnames);
}

public class ItemTypesClassnameImportStore : IClassnameImportStore
{
    private readonly ItemTypesViewModel _viewModel;

    public ItemTypesClassnameImportStore(ItemTypesViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var itemTypes = classnames.Select(name => new ItemType() { Name = name });
        _viewModel.CopyItemTypes(itemTypes);
    }
}

public class TraderCategoryClassnameImportStore : IClassnameImportStore
{
    private readonly TraderCategoryViewModel _viewModel;

    public TraderCategoryClassnameImportStore(TraderCategoryViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Accept(IEnumerable<string> classnames)
    {
        var traderItems = classnames.Select(name => new TraderItem() { Name = name });
        var viewModels = traderItems.Select(item => new TraderItemViewModel(item));

        _viewModel.Items.AddRange(viewModels);
    }
}