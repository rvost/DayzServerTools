using System.Xml;
using System.Xml.Serialization;

using DayzServerTools.Library.Common;

namespace DayzServerTools.Library.Xml;

public abstract class DayzXmlFile<T> : IProjectFile
{
    public void WriteToStream(Stream output)
    {
        var serializer = new XmlSerializer(typeof(T));

        // Remove default namespases
        // See https://stackoverflow.com/questions/760262/xmlserializer-remove-unnecessary-xsi-and-xsd-namespaces
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        // Add an empty namespace and empty value
        ns.Add("", "");

        // Settings for pretty print
        var settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        settings.NewLineOnAttributes = false;

        using var writer = XmlWriter.Create(output, settings);
        // Add standalone="yes" attribute
        writer.WriteProcessingInstruction("xml", @"version=""1.0"" encoding=""UTF-8"" standalone=""yes""");

        serializer.Serialize(writer, this, ns);
    }

    public static T ReadFromStream(Stream input)
    {
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(input);
    }

    public static T ReadFromFile(string filename)
    {
        try
        {
            using var input = File.OpenRead(filename);
            return ReadFromStream(input);
        }
        catch (InvalidOperationException e)
        {
            var originalMessage = e.InnerException?.Message ?? e.Message;
            throw new InvalidOperationException($"{filename}: {originalMessage}", e);
        }
    }
}
