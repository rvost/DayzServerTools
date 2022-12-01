using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.Stores;

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