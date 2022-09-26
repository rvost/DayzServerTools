using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "types")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class ItemTypes: DayzXmlFile<ItemTypes>
{
    [XmlElement("type")]
    public ObservableCollection<ItemType> Types { get; set; } = new ObservableCollection<ItemType>();
}
