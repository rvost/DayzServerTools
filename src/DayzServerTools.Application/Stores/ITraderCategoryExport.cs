using DayzServerTools.Application.ViewModels.Trader;

namespace DayzServerTools.Application.Stores
{
    public interface ITraderCategoryExport
    {
        void ExportTo(TraderCategoryViewModel target);
    }
}
