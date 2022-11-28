using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "economycore")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class EconomyCore : DayzXmlFile<EconomyCore>
{
    [XmlArray("classes")]
    [XmlArrayItem(ElementName = "rootclass")]
    public ObservableCollection<EconomyRootClass> Classes { get; set; } = new();

    [XmlArray("defaults")]
    [XmlArrayItem(ElementName = "default")]
    public ObservableCollection<EconomyDefault> Defaults { get; set; } = new();

    [XmlElement("ce")]
    public ObservableCollection<CeFolder> CeFolders { get; set; } = new();
}
