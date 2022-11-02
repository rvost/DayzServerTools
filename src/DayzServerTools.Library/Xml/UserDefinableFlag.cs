using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

public class UserDefinableFlag : IXmlSerializable, IEquatable<UserDefinableFlag>
{
    public FlagDefinition DefinitionType { get; set; } = FlagDefinition.Vanilla;
    public string Value { get; set; } = "";

    public UserDefinableFlag() { }

    public UserDefinableFlag(FlagDefinition definitionType, string value)
    {
        DefinitionType = definitionType;
        Value = value;
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToContent();

        if (reader.AttributeCount == 0)
        {
            reader.ReadStartElement();
            return;
        }

        reader.MoveToFirstAttribute();
        Value = reader.Value;
        switch (reader.Name)
        {
            case "name":
                DefinitionType = FlagDefinition.Vanilla;
                break;
            case "user":
                DefinitionType = FlagDefinition.User;
                break;
            default:
                throw new InvalidDataException($"{reader.Name} attribute is not supported");
        }

        reader.MoveToElement();
        reader.ReadStartElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            switch (DefinitionType)
            {
                case FlagDefinition.Vanilla:
                    writer.WriteAttributeString("name", Value);
                    break;
                case FlagDefinition.User:
                    writer.WriteAttributeString("user", Value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public override string ToString()
        => $"{DefinitionType}: {Value}";

    public bool Equals(UserDefinableFlag other)
        => DefinitionType == other.DefinitionType && Value == other.Value;

    public override bool Equals(object obj)
    {
        if ((obj == null) || !obj.GetType().Equals(GetType()))
        {
            return false;
        }
        else
        {
            return Equals((UserDefinableFlag)obj);
        }
    }

    public override int GetHashCode()
        => HashCode.Combine(Value, DefinitionType);
    public static bool operator ==(UserDefinableFlag left, UserDefinableFlag rigth)
        => Equals(left, rigth);
    public static bool operator !=(UserDefinableFlag left, UserDefinableFlag rigth)
        => !(left == rigth);
}
