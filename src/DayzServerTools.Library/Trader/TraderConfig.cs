using System.Collections.ObjectModel;

using DayzServerTools.Library.Common;
using DayzServerTools.Library.Trader.Helpers;

namespace DayzServerTools.Library.Trader;

public class TraderConfig: IProjectFile
{
    public CurrencyCategory CurrencyCategory { get; set; } = new();
    public ObservableCollection<Trader> Traders { get; set; } = new();

    public void WriteToStream(Stream output)
        => TraderFileExporter.WriteToStream(output, this);

    public static TraderConfig ReadFromStream(Stream input) 
        => TraderFileParser.ReadFromStream(input);
}
