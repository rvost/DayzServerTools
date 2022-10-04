using CommunityToolkit.Mvvm.Messaging.Messages;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.Messages;

public class LimitsDefinitionsChengedMaessage : PropertyChangedMessage<LimitsDefinitions>
{
    public LimitsDefinitionsChengedMaessage(object sender, string propertyName, LimitsDefinitions oldValue, LimitsDefinitions newValue)
        : base(sender, propertyName, oldValue, newValue)
    {
    }
}
