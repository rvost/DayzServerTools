using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
public class ItemFlags : IXmlSerializable
{
    public bool CountInMap { get; set; } = true;
    public bool CountInHoarder { get; set; } = false;
    public bool CountInCargo { get; set; } = false;
    public bool CountInPlayer { get; set; } = false;
    public bool Crafted { get; set; } = false;
    public bool Deloot { get; set; } = false;

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();

        if (reader.AttributeCount != 6)
            throw new InvalidDataException($"All flags must be defined");

        CountInMap = ReadAttributeAsBool(reader, "count_in_map");
        CountInHoarder = ReadAttributeAsBool(reader, "count_in_hoarder");
        CountInCargo = ReadAttributeAsBool(reader, "count_in_cargo");
        CountInPlayer = ReadAttributeAsBool(reader, "count_in_player");
        Crafted = ReadAttributeAsBool(reader, "crafted");
        Deloot = ReadAttributeAsBool(reader, "deloot");

        reader.ReadStartElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("count_in_cargo", CountInCargo ? "1" : "0");
        writer.WriteAttributeString("count_in_hoarder", CountInHoarder ? "1" : "0");
        writer.WriteAttributeString("count_in_map", CountInMap ? "1" : "0");
        writer.WriteAttributeString("count_in_player", CountInPlayer ? "1" : "0");
        writer.WriteAttributeString("crafted", Crafted ? "1" : "0");
        writer.WriteAttributeString("deloot", Deloot ? "1" : "0");
    }

    private static bool ReadAttributeAsBool(XmlReader reader, string name)
    {
        var value = reader.GetAttribute(name);
        return Convert.ToBoolean(int.Parse(value));
    }
}