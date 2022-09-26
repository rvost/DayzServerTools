using System.Collections.ObjectModel;

namespace DayzServerTools.Library.Trader;

public class Trader
{
    public string TraderName { get; set; } = "";
    public ObservableCollection<TraderCategory> TraderCategories { get; set; } = new();
}
