using System.Collections.ObjectModel;

namespace DayzServerTools.Library.Trader
{
    public class TraderCategory
    {
        public string CategoryName { get; set; } = "";
        public ObservableCollection<TraderItem> TraderItems { get; set; } = new();
    }
}