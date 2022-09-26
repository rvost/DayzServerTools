using System.Collections.ObjectModel;

namespace DayzServerTools.Library.Trader;

public class CurrencyCategory
{
    public string CurrencyName { get; set; } = "";
    public ObservableCollection<CurrencyType> CurrencyTypes { get; set; } = new();
}