using DayzServerTools.Library.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DayzServerTools.Application.ViewModels.Base;

public class ProjectFileViewModelFactory
{
    protected readonly IServiceProvider _serviceProvider;

    public ProjectFileViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ProjectFileViewModel<T> Create<T>(string filename, T model) where T : IProjectFile, new()
    {
        if(string.IsNullOrWhiteSpace(filename)){
            throw new ArgumentException("File name can not be null or whitespace", nameof(filename));
        }

        model ??= new();

        var factory = _serviceProvider.GetRequiredService<IFileViewModelFactory<T>>();
        return factory.Create(filename, model);
    }
}
