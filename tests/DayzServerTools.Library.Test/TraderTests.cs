using DayzServerTools.Library.Trader;

namespace DayzServerTools.Library.Test;

public class TraderTests
{
    [Fact]
    public void CanDeserialize()
    {
        using var input = File.OpenRead("Resources\\TraderConfig.txt");

        var result = TraderConfig.ReadFromStream(input);

        Assert.NotEmpty(result.Traders);
        Assert.Equal(6, result.Traders.Count);
    }
}
