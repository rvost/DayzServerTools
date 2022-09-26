using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "lists")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class LimitsDefinitions : DayzXmlFile<LimitsDefinitions>
{
    [XmlArray("categories")]
    [XmlArrayItem("category")]
    public List<VanillaFlag> Categories { get; set; } = new List<VanillaFlag>();

    [XmlArray("tags")]
    [XmlArrayItem("tag")]
    public List<VanillaFlag> Tags { get; set; } = new List<VanillaFlag>();

    [XmlArray("usageflags")]
    [XmlArrayItem("usage")]
    public List<UserDefinableFlag> UsageFlags { get; set; } = new List<UserDefinableFlag>();

    [XmlArray("valueflags")]
    [XmlArrayItem("value")]
    public List<UserDefinableFlag> ValueFlags { get; set; } = new List<UserDefinableFlag>();
}

