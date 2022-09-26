using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(TypeName = "user_lists")]
[XmlRoot(Namespace = "", IsNullable = false)]
public class UserDefinitions : DayzXmlFile<UserDefinitions>
{
    [XmlArray("usageflags")]
    [XmlArrayItem("user", typeof(UsageUserDefinition))]
    public ObservableCollection<UserDefinition> UsageFlags { get; set; } = new();

    [XmlArray("valueflags")]
    [XmlArrayItem("user", typeof(ValueUserDefinition))]
    public ObservableCollection<UserDefinition> ValueFlags { get; set; } = new();

}

[Serializable]
public abstract class UserDefinition
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "";
    [XmlIgnore]
    public abstract ObservableCollection<UserDefinableFlag> Definitions { get; set; }

    public static explicit operator UserDefinableFlag(UserDefinition obj) => new(FlagDefinition.User, obj.Name);
}

[Serializable]
public class UsageUserDefinition : UserDefinition
{

    [XmlElement("usage", typeof(UserDefinableFlag))]
    public override ObservableCollection<UserDefinableFlag> Definitions { get; set; } = new();
}

[Serializable]
public class ValueUserDefinition : UserDefinition
{
    [XmlElement("value", typeof(UserDefinableFlag))]
    public override ObservableCollection<UserDefinableFlag> Definitions { get; set; } = new();
}
