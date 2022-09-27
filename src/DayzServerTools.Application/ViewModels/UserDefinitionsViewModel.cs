using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Library.Xml;

namespace DayzServerTools.Application.ViewModels;

public partial class UserDefinitionsViewModel : ProjectFileViewModel<UserDefinitions>, IDisposable
{
    [ObservableProperty]
    private WorkspaceViewModel workspace = null;

    public ObservableCollection<UserDefinitionViewModel> ValueFlags { get; } = new();
    public ObservableCollection<UserDefinitionViewModel> UsageFlags { get; } = new();

    public IEnumerable<UserDefinableFlag> AvailableValueFlags { get => Workspace.LimitsDefinitions?.ValueFlags; }
    public IEnumerable<UserDefinableFlag> AvailableUsageFlags { get => Workspace.LimitsDefinitions?.UsageFlags; }

    public IRelayCommand NewValueFlagCommand { get; }
    public IRelayCommand NewUsageFlagCommand { get; }

    public UserDefinitionsViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        Model = new();
        FileName = "cfglimitsdefinitionuser.xml";

        NewValueFlagCommand = new RelayCommand(
            () => ValueFlags.Add(new(new ValueUserDefinition()))
            );
        NewUsageFlagCommand = new RelayCommand(
            () => UsageFlags.Add(new(new UsageUserDefinition()))
            );

        ValueFlags.CollectionChanged += FlagsCollectionChanged;
        UsageFlags.CollectionChanged += FlagsCollectionChanged;
    }

    protected override void OnLoad(Stream input, string filename)
    {
        var userDefinitions = UserDefinitions.ReadFromStream(input);
        ValueFlags.AddRange(
            userDefinitions.ValueFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag))
            );
        UsageFlags.AddRange(
            userDefinitions.UsageFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag))
            );
    }
    protected override IFileDialog CreateOpenFileDialog()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfglimitsdefinitionuser*";
        return dialog;
    }
    protected override bool CanSave()
    {
        var isEmpty = UsageFlags.Count == 0 && ValueFlags.Count == 0;
        var hasErrors = UsageFlags.Any(f => f.HasErrors) || ValueFlags.Any(f => f.HasErrors);
        return !isEmpty && !hasErrors;
    }
   
    private void FlagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ObservableCollection<UserDefinition> target =
            (sender == ValueFlags) ? Model.ValueFlags : Model.UsageFlags;


        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    target.Add(((UserDefinitionViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    target.Remove(((UserDefinitionViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void Dispose()
    {
        ValueFlags.CollectionChanged -= FlagsCollectionChanged;
        UsageFlags.CollectionChanged -= FlagsCollectionChanged;
    }
}
