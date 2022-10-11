using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Stores;

namespace DayzServerTools.Application.ViewModels;

public partial class ClassnamesImportViewModel : ObservableObject
{
    private readonly IClassnameImportStore _importStore;
    [ObservableProperty]
    private string rawInput = "";
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private IEnumerable<string> classnames = new List<string>();

    public IRelayCommand<IEnumerable<string>> ImportCommand { get; }
    public IRelayCommand ParseCommand { get; }
    public event EventHandler CloseRequested;

    public ClassnamesImportViewModel(IClassnameImportStore importStore)
    {
        _importStore = importStore;

        ImportCommand = new RelayCommand<IEnumerable<string>>(Import, CanImport);
        ParseCommand = new RelayCommand(Parse);
    }

    protected bool CanImport(IEnumerable<string> classnames)
        => classnames?.Any() ?? false;
    protected void Import(IEnumerable<string> classnames)
    {
        _importStore.Accept(classnames);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    protected void Parse()
    {
        if (!string.IsNullOrWhiteSpace(rawInput))
        {
            var regex = new Regex(@"\w+");
            Classnames = regex.Matches(RawInput).Select(m => m.Value).ToList();
        }

    }
}
