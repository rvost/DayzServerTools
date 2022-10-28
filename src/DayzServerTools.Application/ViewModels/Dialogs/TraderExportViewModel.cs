using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Stores;
using DayzServerTools.Application.ViewModels.Trader;

namespace DayzServerTools.Application.ViewModels.Dialogs;

public partial class TraderExportViewModel : ObservableObject
{
    private readonly WorkspaceViewModel _workspace;

    public IEnumerable<TraderConfigViewModel> Options { get; }
    public ITraderCategoryExport Store { get; set; }
    public IRelayCommand<TraderCategoryViewModel> ExportCommand { get; }

    public event EventHandler CloseRequested;

    public TraderExportViewModel(WorkspaceViewModel workspace)
    {
        _workspace = workspace;

        Options = _workspace.Tabs
            .Where(tab => tab is TraderConfigViewModel)
            .Select(tab => tab as TraderConfigViewModel)
            .ToList();

        ExportCommand = new RelayCommand<TraderCategoryViewModel>(Export); ;
    }

    public void Export(TraderCategoryViewModel category)
    {
        Store.ExportTo(category);

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
