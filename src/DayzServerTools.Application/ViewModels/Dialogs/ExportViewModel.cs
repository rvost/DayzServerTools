using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DayzServerTools.Application.ViewModels.Dialogs;

public partial class ExportViewModel<T> : ObservableObject, IExportViewModel
{
    private readonly T _exportedObj;

    public IEnumerable<object> Options { get; }

    public event EventHandler CloseRequested;

    public ExportViewModel(T exportedObj, IEnumerable<object> options)
    {
        _exportedObj = exportedObj;
        Options = options;
    }

    public bool CanExport(object param)
        => param is not null && param is IImporter<T>;
    
    [RelayCommand(CanExecute =nameof(CanExport))]
    public void Export(object param)
    {
        if (param is IImporter<T> target)
        {
            target.Import(_exportedObj);
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
