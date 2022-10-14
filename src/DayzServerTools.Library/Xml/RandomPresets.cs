using System.Collections.ObjectModel;
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
    [XmlAttribute("name")]
    public string Name { get; set; }
    [XmlAttribute("chance")]
    public double Chance { get; set; }

    [XmlElement("item")]
    public ObservableCollection<SpawnableItem> Items { get; set; } = new();
}