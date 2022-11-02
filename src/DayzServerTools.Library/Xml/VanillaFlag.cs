using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public class VanillaFlag : IXmlSerializable, IComparable, IComparable<VanillaFlag>, IEquatable<VanillaFlag>
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
        => Value.CompareTo(other?.Value);

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

    public bool Equals(VanillaFlag other)
        => Value == other.Value;

    public override bool Equals(object obj)
    {
        if ((obj == null) || !obj.GetType().Equals(GetType()))
        {
            return false;
        }
        else
        {
            return Equals((VanillaFlag)obj);
        }
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(VanillaFlag left, VanillaFlag rigth)
        => Equals(left, rigth);
    public static bool operator !=(VanillaFlag left, VanillaFlag rigth)
        => !(left == rigth);
}
