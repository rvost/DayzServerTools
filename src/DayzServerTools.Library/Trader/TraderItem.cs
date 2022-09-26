using System.Text.Json.Serialization;

namespace DayzServerTools.Library.Trader;

public class TraderItem
{
    public string Name { get; set; } = "";
    public double SellPrice { get; set; } = -1;
    public double BuyPrice { get; set; } = -1;
    public string Modifier { get; set; } = "*";
}