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
using DayzServerTools.Library.Xml.Validators;
using UserDefinitionsModel = DayzServerTools.Library.Xml.UserDefinitions;

namespace DayzServerTools.Application.ViewModels.UserDefinitions;

public partial class UserDefinitionsViewModel : ProjectFileViewModel<UserDefinitionsModel>, IDisposable
{
    private readonly UserDefinitionsValidator _validator;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableValueFlags), nameof(AvailableUsageFlags))]
    private LimitsDefinitions limitsDefinitions = null;

    public ObservableCollection<UserDefinitionViewModel> ValueFlags { get; } = new();
    public ObservableCollection<UserDefinitionViewModel> UsageFlags { get; } = new();

    public IEnumerable<UserDefinableFlag> AvailableValueFlags
         => limitsDefinitions?.ValueFlags ?? Enumerable.Empty<UserDefinableFlag>();

    public IEnumerable<UserDefinableFlag> AvailableUsageFlags
        => limitsDefinitions?.UsageFlags ?? Enumerable.Empty<UserDefinableFlag>();


    public IRelayCommand NewValueFlagCommand { get; }
    public IRelayCommand NewUsageFlagCommand { get; }
    public IRelayCommand ValidateCommand { get; }

    public UserDefinitionsViewModel(IDialogFactory dialogFactory) : base(dialogFactory)
    {
        _validator = new();
        Model = new();
        FileName = "cfglimitsdefinitionuser.xml";

        NewValueFlagCommand = new RelayCommand(
            () => ValueFlags.Add(new(new ValueUserDefinition(), () => AvailableValueFlags))
            );
        NewUsageFlagCommand = new RelayCommand(
            () => UsageFlags.Add(new(new UsageUserDefinition(), () => AvailableUsageFlags))
            );
        ValidateCommand = new RelayCommand(Validate);

        ValueFlags.CollectionChanged += FlagsCollectionChanged;
        UsageFlags.CollectionChanged += FlagsCollectionChanged;
        WeakReferenceMessenger.Default.Register<UserDefinitionsViewModel, LimitsDefinitionsChengedMaessage>(
            this, (r, m) => r.LimitsDefinitions = m.NewValue
            );
    }

    public void Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        ReportFlagErrors("Usage", UsageFlags);
        ReportFlagErrors("Value", ValueFlags);

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }
    }

    protected override void OnLoad(Stream input, string filename)
    {
        var userDefinitions = UserDefinitionsModel.ReadFromStream(input);
        ValueFlags.AddRange(
            userDefinitions.ValueFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, () => AvailableValueFlags))
            );
        UsageFlags.AddRange(
            userDefinitions.UsageFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, () => AvailableUsageFlags))
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
    private void ReportFlagErrors(string colName, ICollection<UserDefinitionViewModel> flags)
    {
        flags.AsParallel()
            .Select(flag => new { flag.Name, Result = flag.ValidateSelf() })
            .Where(x => !x.Result.IsValid)
            .Select(x => new ValidationErrorInfo(this, $"({colName}):{x.Name}", x.Result.Errors.Select(x => x.ErrorMessage)))
            .ForAll(error => WeakReferenceMessenger.Default.Send(error));
    }

    public void Dispose()
    {
        ValueFlags.CollectionChanged -= FlagsCollectionChanged;
        UsageFlags.CollectionChanged -= FlagsCollectionChanged;
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
