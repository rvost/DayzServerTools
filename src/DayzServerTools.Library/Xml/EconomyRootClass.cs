using System.Xml.Serialization;

namespace DayzServerTools.Library.Xml;

[Serializable]
[XmlType(AnonymousType = true)]
public class EconomyRootClass
{
    private string _ReportMemoryLOD;

    [XmlAttribute("name")]
    public string Name { get; set; } = "";

    [XmlAttribute("act")]
    public string Act { get; set; } = "";
    [XmlIgnore]
    public bool ActSpecified => !string.IsNullOrWhiteSpace(Act);

    [XmlAttribute("reportMemoryLOD")]
    public string SerializableReportMemoryLOD
    {
      get => _ReportMemoryLOD;
      set => _ReportMemoryLOD = value;
    }

    [XmlIgnore]
    public bool ReportMemoryLOD
    {
        get => 
            !string.IsNullOrEmpty(_ReportMemoryLOD) && _ReportMemoryLOD == "yes";
        set
        {
            SerializableReportMemoryLOD = value ? "yes" : "no";
        }
    }
}
