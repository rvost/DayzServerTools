using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Test;

public class UserDefinitionsTests
{
    [Fact]
    public void CanDeserialize()
    {
        var serializer = new XmlSerializer(typeof(UserDefinitions));
        using var input = File.OpenRead("Resources\\cfglimitsdefinitionuser.xml");

        var obj = (UserDefinitions)serializer.Deserialize(input);

        Assert.NotNull(obj);
        Assert.NotEmpty(obj.UsageFlags);
        Assert.NotEmpty(obj.ValueFlags);
    }

    [Fact]
    public void CanSerialize()
    {
        ValidationEventHandler validationHandler = (object sender, ValidationEventArgs args) =>
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                Assert.True(false, args.Message);
            }
        };

        using var schemaReader = new XmlTextReader("Resources\\Schemas\\cfglimitsdefinitionuser.xsd");
        var schema = XmlSchema.Read(schemaReader, validationHandler);

        var serializer = new XmlSerializer(typeof(UserDefinitions));
        using var input = File.OpenRead("Resources\\cfglimitsdefinitionuser.xml");

        var obj = (UserDefinitions)serializer.Deserialize(input);

        using var buffer = new MemoryStream();
        obj.WriteToStream(buffer);

        var doc = new XmlDocument();
        buffer.Position = 0;
        doc.Load(buffer);
        doc.Schemas.Add(schema);
        doc.Validate(validationHandler);
    }
}
