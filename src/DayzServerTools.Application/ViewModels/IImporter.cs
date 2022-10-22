namespace DayzServerTools.Application.ViewModels;

public interface IImporter<T>
{
    void Import(T obj);
}