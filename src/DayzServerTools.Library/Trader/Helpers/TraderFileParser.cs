namespace DayzServerTools.Library.Trader.Helpers;

internal class TraderFileParser
{
    public static TraderConfig ReadFromStream(Stream input)
    {
        var state = new TraderConfig();
        using var fileReader = new StreamReader(input);

        bool endFile = false;
        var lineNumber = 1;
        while (!endFile)
        {
            string fileLine = fileReader.ReadLine().Trim();

            if (!string.IsNullOrEmpty(fileLine) && fileLine.Contains(TraderFileTags.ENDFILE_TAG))
            {
                endFile = true;
            }
            else
            {
                ProcessFileLine(CleanLine(fileLine), state, lineNumber);
                lineNumber++;
            }
        }
        return state;
    }

    private static void ProcessFileLine(string line, TraderConfig parsingState, int lineNumber)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        if (line.Contains(TraderFileTags.CURRENCYNAME_TAG))
        {
            parsingState.CurrencyCategory.CurrencyName = line
                .Substring(TraderFileTags.CURRENCYNAME_TAG.Length, line.Length - TraderFileTags.CURRENCYNAME_TAG.Length)
                .Trim();
        }
        else if (line.Contains(TraderFileTags.CURRENCY_TAG))
        {
            var splitValues = RemoveTag(line, TraderFileTags.CURRENCY_TAG).Split(',');

            if (splitValues.Length < 2)
            {
                throw new InvalidOperationException($"Trader currency config format error on line {lineNumber}");
            }

            parsingState.CurrencyCategory.CurrencyTypes.Add(new CurrencyType
            {
                CurrencyName = splitValues[0].Trim(),
                CurrencyValue = int.Parse(splitValues[1].Trim()),
            });
        }
        else if (line.Contains(TraderFileTags.TRADER_TAG))
        {
            parsingState.Traders.Add(new Trader
            {
                TraderName = RemoveTag(line, TraderFileTags.TRADER_TAG).Trim()
            });
        }
        else if (line.Contains(TraderFileTags.CATEGORY_TAG))
        {
            //As we read sequentially, we always add categories to the last created trader. 
            var parentTrader = parsingState.Traders.Last();
            parentTrader.TraderCategories.Add(new TraderCategory
            {
                CategoryName = RemoveTag(line, TraderFileTags.CATEGORY_TAG).Trim()
            });

        }
        else
        {
            var splitLine = line.Split(',');

            if(splitLine.Length < 4 || splitLine.Any(s => string.IsNullOrWhiteSpace(s)))
            {
                throw new InvalidOperationException($"Trader config format error on line {lineNumber}");
            }

            var parentCategory = parsingState.Traders.Last().TraderCategories.Last();
            var item = new TraderItem
            {
                Name = splitLine[0].Trim(),
                Modifier = splitLine[1].Trim(),
                BuyPrice = int.Parse(splitLine[2].Trim()),
                SellPrice = int.Parse(splitLine[3].Trim())
            };

            parentCategory.TraderItems.Add(item);
        }

    }

    private static string CleanLine(string line)
    {
        var commentIndex = line.IndexOf(TraderFileTags.COMMENT_TAG);

        if (commentIndex >= 0)
        {
            var retLine = line.Substring(0, commentIndex).Trim();

            if (retLine.Length > 0)
                return retLine;
        }
        else
        {
            return line;
        }

        return "";
    }

    private static string RemoveTag(string value, string tag)
    {
        var retLine = value.Substring(tag.Length, value.Length - tag.Length).Trim();

        return string.IsNullOrEmpty(retLine) ? "" : retLine;

    }
}