using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores
{
    public interface ITraderCategoryExport
    {
        void ExportTo(TraderCategoryViewModel target);
    }

    public class ItemTypesToTraderExportStore : ITraderCategoryExport
    {
        private readonly IEnumerable<ItemType> _items;

        public ItemTypesToTraderExportStore(IEnumerable<ItemType> items)
        {
            _items = items;
        }

        public void ExportTo(TraderCategoryViewModel target)
        {
            target.Items.AddRange(
                _items.Select(i => new TraderItemViewModel(new TraderItem { Name = i.Name }))
                );
        }
    }
}
