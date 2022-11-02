using DayzServerTools.Library.Xml;
using System.Collections.ObjectModel;

namespace DayzServerTools.Library.Common;

public interface ILimitsDefinitionsProvider
{
    ObservableCollection<UserDefinableFlag> Usages { get; }
    ObservableCollection<UserDefinableFlag> Values { get; }
    ObservableCollection<VanillaFlag> Categories { get; }
    ObservableCollection<VanillaFlag> Tags { get; }
}
