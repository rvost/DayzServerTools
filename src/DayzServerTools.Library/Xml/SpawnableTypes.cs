using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "spawnabletypes")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class SpawnableTypes : DayzXmlFile<SpawnableTypes>
{
    [XmlElement("type")]
    public ObservableCollection<SpawnableType> Spawnables { get; set; } = new();
}

public class SpawnableType
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("damage")]
    public SpawnDamage Damage { get; set; } = new();
    public bool DamageSpecified 
        => !double.IsNaN(Damage.Min) && !double.IsNaN(Damage.Max);

    [XmlElement("hoarder")]
    public object Hoarder { get; set; }
    public bool HoarderSpecified
       => Hoarder is not null;
    
    [XmlElement("tag")]
    public VanillaFlag Tag { get; set; } = new();
    public bool TagSpecified
        => !string.IsNullOrWhiteSpace(Tag.Value);

    [XmlElement("cargo")]
    public ObservableCollection<SpawnablePreset> Cargo { get; set; } = new();
    [XmlElement("attachments")]
    public ObservableCollection<SpawnablePreset> Attachments { get; set; } = new();
}

public class SpawnDamage
{
    [XmlAttribute("min")]
    public double Min { get; set; }
    [XmlAttribute("max")]
    public double Max { get; set; }

    public SpawnDamage(double min, double max)
    {
        Min = min;
        Max = max;
    }
    public SpawnDamage() : this(double.NaN, double.NaN) { }
}

public class SpawnablePreset
{
    [XmlAttribute("preset")]
    public string Preset { get; set; } = "";
    public bool PresetSpecified
        => !string.IsNullOrWhiteSpace(Preset);

    [XmlAttribute("chance")]
    public double Chance { get; set; } = 0;
    public bool ChanceSpecified
        => !PresetSpecified;

    [XmlElement("item")]
    public ObservableCollection<SpawnableItem> Items { get; set; } = new();
    public bool ItemsSpecified
        => !PresetSpecified;
}