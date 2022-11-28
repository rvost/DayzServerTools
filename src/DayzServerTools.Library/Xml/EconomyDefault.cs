using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public class EconomyDefault
{
    [XmlAttribute("name")]
    public string Name { get; set; }
    [XmlAttribute("value")]
    public string Value { get; set; }
}