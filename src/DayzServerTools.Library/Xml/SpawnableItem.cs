using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public class SpawnableItem
{
    [XmlAttribute("name")]
    public string Name { get; set; }
    [XmlAttribute("chance")]
    public double Chance { get; set; }
    public bool ChanceSpecified => !double.IsNaN(Chance);

    public SpawnableItem(string name, double chance)
    {
        Name = name;
        Chance = chance;
    }

    public SpawnableItem() : this("", double.NaN) { }
}