using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ItemTypesModel = DayzServerTools.Library.Xml.ItemTypes;

namespace DayzServerTools.Application.ViewModels.ItemTypes;

public class ItemTypesViewModelsFactory : IFileViewModelFactory<ItemTypesModel>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDispatcherService _dispatcherService;
    private readonly ILimitsDefinitionsProvider _limitsDefinitions;

    public ItemTypesViewModelsFactory(IDispatcherService dispatcherService, ILimitsDefinitionsProvider limitsDefinitions,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dispatcherService = dispatcherService;
        _limitsDefinitions = limitsDefinitions;
    }

    public ItemTypeViewModel Create(ItemType model)
    {
        var validator = new ItemTypeValidator(_limitsDefinitions);
        return new ItemTypeViewModel(model, validator, _dispatcherService);
    }

    public ProjectFileViewModel<ItemTypesModel> Create(string filename, ItemTypesModel model)
    {
        var dialogFactory = _serviceProvider.GetRequiredService<IDialogFactory>();
        var validator = _serviceProvider.GetRequiredService<IValidator<ItemTypesModel>>();
        var workspace = _serviceProvider.GetRequiredService<WorkspaceViewModel>();

        return new ItemTypesViewModel(filename, model, dialogFactory, validator, workspace, this);
    }
}
