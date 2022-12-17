namespace DayzServerTools.Application.Models;

public class OpenFileDialogOptions
{
    private const string _xmlFilter = "XML Files (*.xml)|*.xml";
    private const string _txtFilter = "Text|*.txt";

    public string FileName { get;}
    public string Filter { get;}

    public OpenFileDialogOptions(string fileName, string filter)
    {
        FileName = fileName;
        Filter = filter;
    }

    public static OpenFileDialogOptions TypesOptions { get; } = new("*", _xmlFilter);
    public static OpenFileDialogOptions RandomPresetsOptions { get; } = new("cfgrandompresets*", _xmlFilter);
    public static OpenFileDialogOptions SpawnableTypesOptions { get; } = new("*", _xmlFilter);
    public static OpenFileDialogOptions TraderConfigOptions { get; } = new("TraderConfig*", _txtFilter);
    public static OpenFileDialogOptions UserDefinitionsOptions { get; } = new("cfglimitsdefinitionuser*", _xmlFilter);
}
