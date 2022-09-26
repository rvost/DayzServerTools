using System.Collections.ObjectModel;

namespace DayzServerTools.Library.Trader.Helpers;

internal class TraderFileExporter
{
    public static void WriteToStream(Stream output, TraderConfig data)
    {
        using var fileWriter = new StreamWriter(output);

        fileWriter.Write(ExportCurrencyString(data.CurrencyCategory));
        WriteTraderData(fileWriter, data.Traders);
        fileWriter.Write(TraderFileTags.ENDFILE_TAG);
    }

    private static void WriteTraderData(StreamWriter writer, ObservableCollection<Trader> traderData)
    {
        foreach (var trader in traderData)
        {
            writer.Write($"{TraderFileTags.TRADER_TAG} {trader.TraderName}\n");


            trader.TraderCategories.ToList().ForEach(cat =>
            {
                writer.Write(ExportTraderCategoryString(cat));
            });
        }
    }

    private static string ExportCurrencyString(CurrencyCategory currencyCategory)
    {
        try
        {
            string retString = $"{TraderFileTags.CURRENCYNAME_TAG} {currencyCategory.CurrencyName} \n";

            foreach (var currency in currencyCategory.CurrencyTypes)
            {
                retString += $"\t{TraderFileTags.CURRENCY_TAG} {currency.CurrencyName},\t\t{currency.CurrencyValue}\n";
            }

            return retString + "\n";
        }
        catch (Exception)
        {
            return "";
        }
    }

    private static string ExportTraderCategoryString(TraderCategory traderCategory)
    {

        var retString = $"\t{TraderFileTags.CATEGORY_TAG} {traderCategory.CategoryName}\n";

        traderCategory.TraderItems.ToList().ForEach(item =>
        {
            retString += $"\t\t{item.Name},\t\t\t{item.Modifier},\t\t{item.BuyPrice},\t\t{item.SellPrice}\n";
        });

        return retString + "\n";
    }
}
