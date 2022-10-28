using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.Trader;

namespace DayzServerTools.Application.Stores;

public class TraderItemsExportStore : ITraderCategoryExport
{
    private readonly IEnumerable<TraderItemViewModel> _items;
    private readonly bool _copy;
    public TraderItemsExportStore(IEnumerable<TraderItemViewModel> items, bool copy)
    {
        _items = items;
        _copy = copy;
    }

    public void ExportTo(TraderCategoryViewModel target)
    {
        var source = _copy ? _items.Select(item => item.Copy()) : _items;
        target.Items.AddRange(source);
    }
}
