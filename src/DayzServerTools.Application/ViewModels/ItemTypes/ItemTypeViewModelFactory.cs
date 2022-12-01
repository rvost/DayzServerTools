using DayzServerTools.Application.Services;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validators;

namespace DayzServerTools.Application.ViewModels.ItemTypes;

public class ItemTypeViewModelFactory
{
    private readonly IDispatcherService _dispatcherService;
    private readonly ILimitsDefinitionsProvider _limitsDefinitions;

    public ItemTypeViewModelFactory(IDispatcherService dispatcherService, ILimitsDefinitionsProvider limitsDefinitions)
	{
        _dispatcherService = dispatcherService;
        _limitsDefinitions = limitsDefinitions;
    }

    public ItemTypeViewModel Create(ItemType model)
    {
        var validator = new ItemTypeValidator(_limitsDefinitions);
        return new ItemTypeViewModel(model, validator, _dispatcherService);
    }
}
