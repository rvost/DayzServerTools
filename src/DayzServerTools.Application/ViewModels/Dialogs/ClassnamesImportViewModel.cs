using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Stores;

namespace DayzServerTools.Application.ViewModels;

public partial class ClassnamesImportViewModel : ObservableObject
{
    private readonly IClassnameImportStore _importStore;
   
    private string rawInput = "";
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private IEnumerable<string> classnames = new List<string>();

    public string RawInput
    {
        get => rawInput;
        set
        {
            SetProperty(ref rawInput, value);
            Classnames = Parse(value);
        }
    }

    public event EventHandler CloseRequested;

    public ClassnamesImportViewModel(IClassnameImportStore importStore)
    {
        _importStore = importStore;
    }

    protected bool CanImport(IEnumerable<string> classnames)
        => classnames?.Any() ?? false;

    [RelayCommand(CanExecute =nameof(CanImport))]
    protected void Import(IEnumerable<string> classnames)
    {
        _importStore.Accept(classnames);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    protected IEnumerable<string> Parse(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            var regex = new Regex(@"\w+");
            return regex.Matches(RawInput).Select(m => m.Value).ToList();
        }
        else
        {
            return Enumerable.Empty<string>();
        }
    }
}
