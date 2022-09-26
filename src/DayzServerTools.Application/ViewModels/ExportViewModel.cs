using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public partial class ExportViewModel : ObservableObject
{
    private readonly WorkspaceViewModel _workspace;

    public IEnumerable<TraderConfigViewModel> Options { get; }
    public IEnumerable<ItemType> Items { get; set; }
    public IRelayCommand<TraderCategory> ExportCommand { get; }
    
    public event EventHandler CloseRequested;

    public ExportViewModel(WorkspaceViewModel workspace)
    {
        _workspace = workspace;

        Options = _workspace.Tabs
            .Where(tab => tab is TraderConfigViewModel)
            .Select(tab => tab as TraderConfigViewModel)
            .ToList();

        ExportCommand = new RelayCommand<TraderCategory>(Export); ;
    }

    public void Export(TraderCategory category)
    {
        category?.TraderItems.AddRange(
            Items.Select(i => new TraderItem { Name = i.Name })
            );

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
