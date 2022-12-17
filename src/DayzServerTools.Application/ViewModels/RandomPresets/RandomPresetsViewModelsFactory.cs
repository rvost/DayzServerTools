using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;

using RandomPresetsModel = DayzServerTools.Library.Xml.RandomPresets;

namespace DayzServerTools.Application.ViewModels.RandomPresets;

public class RandomPresetsViewModelsFactory : IFileViewModelFactory<RandomPresetsModel>
{
    private readonly IServiceProvider _serviceProvider;

    public RandomPresetsViewModelsFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ProjectFileViewModel<RandomPresetsModel> Create(string filename, RandomPresetsModel model)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<RandomPresetsModel>>();
        var dialogFactory = _serviceProvider.GetRequiredService<IDialogFactory>();

        return new RandomPresetsViewModel(filename, model, validator, dialogFactory);
    }
}
