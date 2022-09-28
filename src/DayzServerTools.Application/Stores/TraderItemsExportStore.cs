using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.Stores;

public class TraderItemsExportStore : ITraderCategoryExport
{
    private readonly IEnumerable<TraderItem> _items;
    private readonly bool _copy;
    public TraderItemsExportStore(IEnumerable<TraderItem> items, bool copy)
    {
        _items = items;
        _copy = copy;
    }

    public void ExportTo(TraderCategory target)
    {
        var source = _copy ? _items.Select(item => item.Copy()) : _items;
        target.TraderItems.AddRange(source);
    }
}
