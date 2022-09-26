using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public class VanillaFlag : IXmlSerializable, IComparable, IComparable<VanillaFlag>
{
    public string Value { get; set; } = "";

    public VanillaFlag() { }
    public VanillaFlag(string value)
    {
        Value = value;
    }

    public static implicit operator VanillaFlag(string val) => new(val);
    public static explicit operator string(VanillaFlag obj) => obj.Value;

    public override string ToString() => Value;

    public int CompareTo(VanillaFlag other)
    {
        return Value.CompareTo(other?.Value);
    }

    public int CompareTo(object obj)
    {
        if (obj is null)
        {
            return 1;
        }
        else if (obj is VanillaFlag that)
        {
            return Value.CompareTo(that.Value);
        }
        else
        {
            throw new ArgumentException(nameof(obj));
        }
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();

        if (!reader.HasAttributes)
        {
            reader.ReadStartElement();
            return;
        }

        var value = reader.GetAttribute("name");
        if (value is null)
        {
            throw new InvalidDataException($"{reader.Name} has unsupported attributes");
        }
        Value = value;
        reader.ReadStartElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            writer.WriteAttributeString("name", Value);
        }
    }
}
