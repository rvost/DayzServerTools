namespace DayzServerTools.Application.Stores;

public interface IClassnameImportStore
{
    void Accept(IEnumerable<string> classnames);
}
