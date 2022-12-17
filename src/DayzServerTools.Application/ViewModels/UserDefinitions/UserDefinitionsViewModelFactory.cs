using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;

using UserDefinitionsModel = DayzServerTools.Library.Xml.UserDefinitions;

namespace DayzServerTools.Application.ViewModels.UserDefinitions;

public class UserDefinitionsViewModelFactory: IFileViewModelFactory<UserDefinitionsModel>
{
    private readonly IServiceProvider _serviceProvider;

    public UserDefinitionsViewModelFactory(IServiceProvider serviceProvider)
	{
        _serviceProvider = serviceProvider;
    }

    public ProjectFileViewModel<UserDefinitionsModel> Create(string filename, UserDefinitionsModel model)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<UserDefinitionsModel>>();
        var dialogFactory = _serviceProvider.GetRequiredService<IDialogFactory>();
        return new UserDefinitionsViewModel(filename, model, validator, dialogFactory);
    }
}
