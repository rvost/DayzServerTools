using CommunityToolkit.Mvvm.Input;

namespace DayzServerTools.Application.ViewModels.Dialogs;

public interface IExportViewModel
{
    public IRelayCommand<object> ExportCommand { get; }
    public event EventHandler CloseRequested;
}
