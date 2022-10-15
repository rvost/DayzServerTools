using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Test;

public class SpawnableTypesTests
{
    [Fact]
    public void CanDeserialize()
    {
        using var input = File.OpenRead("Resources\\cfgspawnabletypes.xml");

        var obj = SpawnableTypes.ReadFromStream(input);

        Assert.NotNull(obj);
        Assert.Equal(506, obj.Spawnables.Count);
    }

    [Fact]
    public void TrySerialize()
    {
        var first = new SpawnableType() { Name = "Barrel_Blue", Hoarder = new() };
        var second = new SpawnableType() { Name = "GiftWrapPaper", Damage = new(0, 0.32) };

        var a_1 = new SpawnablePreset()
        {
            Chance = 0.85,
            Items = new(new List<SpawnableItem> { new("PlateCarrierHolster_Camo", 1) })
        };
        var a_2 = new SpawnablePreset()
        {
            Chance = 0.85,
            Items = new(new List<SpawnableItem> { new("PlateCarrierPouches_Camo", 1) })
        };
        var third = new SpawnableType()
        {
            Name = "PlateCarrierVest_Camo",
            Damage = new(0.1, 0.6),
            Attachments = new(new List<SpawnablePreset> { a_1, a_2 })
        };

        var obj = new SpawnableTypes();
        obj.Spawnables.Add(first);
        obj.Spawnables.Add(second);
        obj.Spawnables.Add(third);

        using var output = File.Create("serialized.xml");
        obj.WriteToStream(output);
    }
}
