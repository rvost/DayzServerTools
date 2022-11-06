namespace DayzServerTools.Library.Common;

public interface IRandomPresetsProvider
{
    IEnumerable<string> AvailableCargoPresets { get; }
    IEnumerable<string> AvailableAttachmentsPresets { get; }
}
