using DayzServerTools.Application.Services;
using DayzServerTools.Library.Trader;
using TraderModel = DayzServerTools.Library.Trader.Trader;

namespace DayzServerTools.Application.ViewModels.Trader;

public class TraderViewModelsFactory
{
    private readonly IDialogFactory _dialogFactory;
    private readonly WorkspaceViewModel _workspace;

    public TraderViewModelsFactory(IDialogFactory dialogFactory, WorkspaceViewModel workspace)
    {
        _dialogFactory = dialogFactory;
        _workspace = workspace;
    }

    public TraderViewModel CreateTraderViewModel(TraderModel model)
        => new(model, this);

    public TraderCategoryViewModel CreateTraderCategoryViewModel(TraderCategory model)
        => new(model, _dialogFactory, _workspace);
}
