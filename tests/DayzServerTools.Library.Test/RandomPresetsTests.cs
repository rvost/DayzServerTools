using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Test;

public class RandomPresetsTests
{
    [Fact]
    public void CanDeserialize()
    {
        using var input = File.OpenRead("Resources\\cfgrandompresets.xml");

        var obj = RandomPresets.ReadFromStream(input);

        Assert.NotNull(obj);

        Assert.Equal(43, obj.CargoPresets.Count);
        Assert.Equal(35, obj.AttachmentsPresets.Count);
    }

    [Fact]
    public void TrySerialize()
    {
        var cargo = new RandomPreset() { Name = "foodHermit", Chance = 0.15 };
        cargo.Items.Add(new ("TunaCan", 0.11));
        cargo.Items.Add(new ("SardinesCan", 0.11));
        cargo.Items.Add(new ("Apple", 0.07));
        
        var attachments = new RandomPreset() { Name= "bagsHunter", Chance=0.10};
        attachments.Items.Add(new("CourierBag", 0.10));
        attachments.Items.Add(new("HuntingBag", 0.50));

        var obj = new RandomPresets();
        obj.CargoPresets.Add(cargo);
        obj.AttachmentsPresets.Add(attachments);

        using var output = File.Create("serialized.xml");
        obj.WriteToStream(output);
    }
}
