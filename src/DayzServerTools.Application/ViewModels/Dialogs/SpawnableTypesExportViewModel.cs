using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace DayzServerTools.Application.ViewModels.Dialogs;

public partial class SpawnableTypesExportViewModel : ObservableObject
{
    private readonly WorkspaceViewModel _workspace;
    private readonly IEnumerable<string> _classnames;
    [ObservableProperty]
    private object selectedItem;

    public IEnumerable<SpawnableTypesViewModel> Options { get; }

    public IRelayCommand<object> ExportCommand { get; }

    public event EventHandler CloseRequested;

    public SpawnableTypesExportViewModel(IEnumerable<string> classnames)
    {
        _workspace = Ioc.Default.GetRequiredService<WorkspaceViewModel>();
        _classnames = classnames;

        Options = _workspace.Tabs
           .Where(tab => tab is SpawnableTypesViewModel)
           .Select(tab => tab as SpawnableTypesViewModel)
           .ToList();

        ExportCommand = new RelayCommand<object>(Export, CanExport);
    }

    public bool CanExport(object param)
        => param is not null && param is IClassnamesImporter;
    public void Export(object param)
    {
        if (param is IClassnamesImporter target)
        {
            target.AcceptClassnames(_classnames);
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
