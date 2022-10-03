using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
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
    public IRelayCommand ValidateCommand { get; }

    public UserDefinitionsViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        Model = new();
        FileName = "cfglimitsdefinitionuser.xml";

        NewValueFlagCommand = new RelayCommand(
            () => ValueFlags.Add(new(new ValueUserDefinition(), AvailableValueFlags))
            );
        NewUsageFlagCommand = new RelayCommand(
            () => UsageFlags.Add(new(new UsageUserDefinition(), AvailableUsageFlags))
            );
        ValidateCommand = new RelayCommand(Validate);

        ValueFlags.CollectionChanged += FlagsCollectionChanged;
        UsageFlags.CollectionChanged += FlagsCollectionChanged;
    }

    public void Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        ReportFlagErrors(UsageFlags);
        ReportFlagErrors(ValueFlags);
    }

    protected override void OnLoad(Stream input, string filename)
    {
        var userDefinitions = UserDefinitions.ReadFromStream(input);
        ValueFlags.AddRange(
            userDefinitions.ValueFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, AvailableValueFlags))
            );
        UsageFlags.AddRange(
            userDefinitions.UsageFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, AvailableUsageFlags))
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
    private void ReportFlagErrors(ICollection<UserDefinitionViewModel> flags)
    {
        flags.AsParallel().ForAll(flag => flag.ValidateSelf());

        var allErrors = flags.AsParallel()
            .Where(flag => flag.HasErrors)
            .Select(flag =>
            {
                var errorMessages = flag.GetErrors().Select(r => r.ErrorMessage);
                return new ValidationErrorInfo(this, flag.Name, errorMessages);
            }
            );
        
        allErrors.ForAll(error => WeakReferenceMessenger.Default.Send(error));
    }
    
    public void Dispose()
    {
        ValueFlags.CollectionChanged -= FlagsCollectionChanged;
        UsageFlags.CollectionChanged -= FlagsCollectionChanged;
    }
}
