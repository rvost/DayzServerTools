using CommunityToolkit.Mvvm.Input;

namespace DayzServerTools.Application.ViewModels.Base;

public interface IProjectFileTab
{
    string Name { get; }
    string FileName { get; }
    IRelayCommand SaveCommand { get; }
    IRelayCommand SaveAsCommand { get; }
    IRelayCommand CloseCommand { get; }
    IRelayCommand ValidateCommand { get; }
    event EventHandler CloseRequested;
}
