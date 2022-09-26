using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Library.Test
{

    public class LimitsDefinitionsTests
    {
        [Fact]
        public void CanDeserialize()
        {
            var serializer = new XmlSerializer(typeof(LimitsDefinitions));
            using var input = File.OpenRead("Resources\\cfglimitsdefinition.xml");

            var obj = (LimitsDefinitions)serializer.Deserialize(input);

            Assert.NotNull(obj);

            Assert.Equal(10, obj.Categories.Count);
            Assert.Equal(11, obj.Tags.Count);
            Assert.Equal(2, obj.UsageFlags.Count);
            Assert.Equal(6, obj.ValueFlags.Count);
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

            using var schemaReader = new XmlTextReader("Resources\\Schemas\\cfglimitsdefinition.xsd");
            var schema = XmlSchema.Read(schemaReader, validationHandler);

            var obj = new LimitsDefinitions()
            {
                Categories = { "rifles", "lore", "food" },
                Tags = { "military", "hunting" },
                UsageFlags = new List<UserDefinableFlag>(),
                ValueFlags = {
                    new UserDefinableFlag(FlagDefinition.Vanilla, "Tier1"),
                    new UserDefinableFlag(FlagDefinition.Vanilla, "Tier2")
                }
            };
            var serializer = new XmlSerializer(typeof(LimitsDefinitions));
            using var buffer = new MemoryStream();
            obj.WriteToStream(buffer);

            var doc = new XmlDocument();
            buffer.Position = 0;
            doc.Load(buffer);
            doc.Schemas.Add(schema);
            doc.Validate(validationHandler);
        }
    }
}
