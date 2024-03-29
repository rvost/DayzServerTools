﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Xml;
using UserDefinitionsModel = DayzServerTools.Library.Xml.UserDefinitions;

namespace DayzServerTools.Application.ViewModels.UserDefinitions;

public partial class UserDefinitionsViewModel : ProjectFileViewModel<UserDefinitionsModel>, IDisposable
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableValueFlags), nameof(AvailableUsageFlags))]
    private LimitsDefinitions limitsDefinitions = null;

    public ObservableCollection<UserDefinitionViewModel> ValueFlags { get; } = new();
    public ObservableCollection<UserDefinitionViewModel> UsageFlags { get; } = new();

    public IEnumerable<UserDefinableFlag> AvailableValueFlags
         => limitsDefinitions?.ValueFlags ?? Enumerable.Empty<UserDefinableFlag>();

    public IEnumerable<UserDefinableFlag> AvailableUsageFlags
        => limitsDefinitions?.UsageFlags ?? Enumerable.Empty<UserDefinableFlag>();

    public UserDefinitionsViewModel(string fileName, UserDefinitionsModel model, IValidator<UserDefinitionsModel> validator,
        IDialogFactory dialogFactory) : base(fileName, model, validator, dialogFactory)
    {
        ValueFlags.AddRange(
            model.ValueFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, () => AvailableValueFlags))
            );
        UsageFlags.AddRange(
            model.UsageFlags.Select<UserDefinition, UserDefinitionViewModel>(flag => new(flag, () => AvailableUsageFlags))
            );

        ValueFlags.CollectionChanged += FlagsCollectionChanged;
        UsageFlags.CollectionChanged += FlagsCollectionChanged;
        WeakReferenceMessenger.Default.Register<UserDefinitionsViewModel, LimitsDefinitionsChengedMaessage>(
            this, (r, m) => r.LimitsDefinitions = m.NewValue
            );
    }

    protected override bool Validate()
    {
        WeakReferenceMessenger.Default.Send(new ClearValidationErrorsMessage(this));

        bool usagesHaveErrors = ReportFlagErrors("Usage", UsageFlags);
        bool valuesHaveErrors = ReportFlagErrors("Value", ValueFlags);

        var res = _validator.Validate(Model);
        if (!res.IsValid)
        {
            res.Errors.AsParallel()
                .Select(error => new ValidationErrorInfo(this, "", new[] { error.ErrorMessage }))
                .ForAll(error => WeakReferenceMessenger.Default.Send(error));
        }

        return res.IsValid && !usagesHaveErrors && !valuesHaveErrors;
    }

    [RelayCommand]
    private void NewValueFlag()
        => ValueFlags.Add(new(new ValueUserDefinition(), () => AvailableValueFlags));
    
    [RelayCommand]
    private void NewUsageFlag()
        => UsageFlags.Add(new(new UsageUserDefinition(), () => AvailableUsageFlags));

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
    private bool ReportFlagErrors(string colName, ICollection<UserDefinitionViewModel> flags)
    {
        var errorInfos = flags.AsParallel()
            .Select(flag => new { flag.Name, Result = flag.ValidateSelf() })
            .Where(x => !x.Result.IsValid)
            .Select(x => new ValidationErrorInfo(this, $"({colName}):{x.Name}", x.Result.Errors.Select(x => x.ErrorMessage)))
            .ToList();

        errorInfos.AsParallel().ForAll(error => WeakReferenceMessenger.Default.Send(error));

        return errorInfos.Any();
    }

    public void Dispose()
    {
        ValueFlags.CollectionChanged -= FlagsCollectionChanged;
        UsageFlags.CollectionChanged -= FlagsCollectionChanged;
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
