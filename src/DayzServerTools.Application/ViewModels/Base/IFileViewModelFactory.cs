using DayzServerTools.Library.Common;

namespace DayzServerTools.Application.ViewModels.Base;

public interface IFileViewModelFactory<T> where T : IProjectFile
{
    ProjectFileViewModel<T> Create(string filename, T model);
}