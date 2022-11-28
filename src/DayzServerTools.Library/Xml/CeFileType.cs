using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public enum CeFileType
{
    [XmlEnum(Name = "types")]
    Types,
    [XmlEnum(Name = "spawnabletypes")]
    SpawnableTypes,
    [XmlEnum(Name = "globals")]
    Globals,
    [XmlEnum(Name = "economy")]
    Economy,
    [XmlEnum(Name = "events")]
    Events,
    [XmlEnum(Name = "messages")]
    Messages
}
