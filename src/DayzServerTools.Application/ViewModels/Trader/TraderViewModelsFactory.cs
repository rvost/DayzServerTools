using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Trader;

using TraderModel = DayzServerTools.Library.Trader.Trader;

namespace DayzServerTools.Application.ViewModels.Trader;

public class TraderViewModelsFactory: IFileViewModelFactory<TraderConfig>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDialogFactory _dialogFactory;
    private readonly WorkspaceViewModel _workspace;

    public TraderViewModelsFactory(IDialogFactory dialogFactory, WorkspaceViewModel workspace, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _dialogFactory = dialogFactory;
        _workspace = workspace;
    }

    public TraderViewModel CreateTraderViewModel(TraderModel model)
        => new(model, this);

    public TraderCategoryViewModel CreateTraderCategoryViewModel(TraderCategory model)
        => new(model, _dialogFactory, _workspace);

    public ProjectFileViewModel<TraderConfig> Create(string filename, TraderConfig model)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<TraderConfig>>();
        return new TraderConfigViewModel(filename, model,validator, _dialogFactory, this);
    }
}
