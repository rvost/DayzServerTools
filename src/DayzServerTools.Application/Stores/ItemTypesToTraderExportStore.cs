using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Stores
{
    public interface ITraderCategoryExport
    {
        void ExportTo(TraderCategory target);
    }

    public class ItemTypesToTraderExportStore : ITraderCategoryExport
    {
        private readonly IEnumerable<ItemType> _items;

        public ItemTypesToTraderExportStore(IEnumerable<ItemType> items)
        {
            _items = items;
        }

        public void ExportTo(TraderCategory target)
        {
            target.TraderItems.AddRange(
                _items.Select(i => new TraderItem { Name = i.Name })
                );
        }
    }
}
