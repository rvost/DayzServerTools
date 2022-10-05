namespace DayzServerTools.Application.ViewModels.Panes;

public interface IPane
{
    string Title { get; }

    bool IsVisible { get; set; }
    bool IsSelected { get; set; }
}
