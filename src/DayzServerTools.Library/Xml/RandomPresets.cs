using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "randompresets")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class RandomPresets : DayzXmlFile<RandomPresets>
{
    [XmlElement("cargo")]
    public ObservableCollection<RandomPreset> CargoPresets { get; set; } = new();
    [XmlElement("attachments")]
    public ObservableCollection<RandomPreset> AttachmentsPresets { get; set; } = new();
}

public class RandomPreset
{
    [XmlAttribute("chance")]
    public string ChanceFormat
    {
        get => Chance.ToString(Chance < 1 ? "0.00" : "0.0", CultureInfo.InvariantCulture);
        set => Chance = double.Parse(value, CultureInfo.InvariantCulture);
    }
    [XmlAttribute("name")]
    public string Name { get; set; }
    [XmlIgnore]
    public double Chance { get; set; }

    [XmlElement("item")]
    public ObservableCollection<SpawnableItem> Items { get; set; } = new();
}