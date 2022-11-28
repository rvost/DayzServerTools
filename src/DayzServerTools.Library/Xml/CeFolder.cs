using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(AnonymousType = true)]
public class CeFolder
{
    [XmlAttribute("folder")]
    public string Folder { get; set; }

    [XmlElement("file")]
    public ObservableCollection<CeFile> Files { get; set; } = new();
}
