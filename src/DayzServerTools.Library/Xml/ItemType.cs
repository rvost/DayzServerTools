using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(AnonymousType = true)]
public class ItemType
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "";

    [XmlElement("nominal", typeof(int))]
    public int Nominal { get; set; } = 0;

    [XmlElement("lifetime", typeof(int))]
    public int Lifetime { get; set; } = 3600;

    [XmlElement("restock", typeof(int))]
    public int Restock { get; set; } = 1800;

    [XmlElement("min", typeof(int))]
    public int Min { get; set; } = 0;

    [XmlElement("quantmin", typeof(int))]
    public int Quantmin { get; set; } = -1;

    [XmlElement("quantmax", typeof(int))]
    public int Quantmax { get; set; } = -1;

    [XmlElement("cost", typeof(int))]
    public int Cost { get; set; } = 100;

    [XmlElement("flags", typeof(ItemFlags))]
    public ItemFlags Flags { get; set; } = new ItemFlags();

    [XmlElement("usage", typeof(UserDefinableFlag))]
    public ObservableCollection<UserDefinableFlag> Usages { get; set; } = new ObservableCollection<UserDefinableFlag>();

    [XmlElement("value", typeof(UserDefinableFlag))]
    public ObservableCollection<UserDefinableFlag> Value { get; set; } = new ObservableCollection<UserDefinableFlag>();

    [XmlElement("category", typeof(VanillaFlag))]
    public VanillaFlag Category { get; set; } = new VanillaFlag();

    [XmlIgnore]
    public bool CategorySpecified
    {
        get => !string.IsNullOrEmpty(Category?.Value);
        set { return; }
    }

    [XmlElement("tag", typeof(VanillaFlag))]
    public ObservableCollection<VanillaFlag> Tags { get; set; } = new ObservableCollection<VanillaFlag>();
}
