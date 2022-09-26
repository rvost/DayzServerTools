using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Test;

public class TypesXmlTests
{
    [Theory]
    [InlineData("Resources\\Types\\sampleSingle.xml")]
    [InlineData("Resources\\Types\\sampleChernarusplus.xml")]
    [InlineData("Resources\\Types\\sampleNamalsk.xml")]
    [InlineData("Resources\\Types\\sampleJoined.xml")]
    public void CanDeserialize(string filePath)
    {
        var serializer = new XmlSerializer(typeof(ItemTypes));
        using var reader = File.OpenRead(filePath);

        var o = (ItemTypes)serializer.Deserialize(reader);

        Assert.NotNull(o);
        Assert.NotEmpty(o.Types);
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

        using var schemaReader = new XmlTextReader("Resources\\Schemas\\types.xsd");
        var schema = XmlSchema.Read(schemaReader, validationHandler);

        var obj = new ItemTypes
        {
            Types =
            {
                new ItemType
                {
                    Name = "AK74",
                    Nominal = 3,
                    Min = 2,
                    Flags = new ItemFlags { CountInMap =true },
                    Value ={ new UserDefinableFlag(FlagDefinition.User, "Tier4") },
                    Category = "rifles",
                    Tags =  {"military"}
                }
            }
        };

        using var buffer = new MemoryStream();
        obj.WriteToStream(buffer);

        var doc = new XmlDocument();
        buffer.Position = 0;
        doc.Load(buffer);
        doc.Schemas.Add(schema);
        doc.Validate(validationHandler);
    }
}