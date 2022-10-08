using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Services;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Xml.Validation;

namespace DayzServerTools.Application.ViewModels;

public class ItemTypeViewModel : ObservableValidator
{
    private readonly IDispatcherService _dispatcher;
    private ItemType _model;

    public ItemType Model { get => _model; }

    [Required]
    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, _model, (m, v) => m.Name = v, true);
    }

    [Range(0, int.MaxValue)]
    public int Nominal
    {
        get => _model.Nominal;
        set => SetProperty(_model.Nominal, value, _model, (m, v) => m.Nominal = v, true);
    }

    [Range(0, 3888000)]
    public int Lifetime
    {
        get => _model.Lifetime;
        set => SetProperty(_model.Lifetime, value, _model, (m, v) => m.Lifetime = v, true);
    }

    [Range(0, 3888000)]
    public int Restock
    {
        get => _model.Restock;
        set => SetProperty(_model.Restock, value, _model, (m, v) => m.Restock = v, true);
    }

    [Range(0, int.MaxValue)]
    public int Min
    {
        get => _model.Min;
        set => SetProperty(_model.Min, value, _model, (m, v) => m.Min = v, true);
    }

    [Range(-1, 100)]
    public int Quantmin
    {
        get => _model.Quantmin;
        set => SetProperty(_model.Quantmin, value, _model, (m, v) => m.Quantmin = v, true);
    }

    [Range(-1, 100)]
    public int Quantmax
    {
        get => _model.Quantmax;
        set => SetProperty(_model.Quantmax, value, _model, (m, v) => m.Quantmax = v, true);
    }

    [Range(0, 100)]
    public int Cost
    {
        get => _model.Cost;
        set => SetProperty(_model.Cost, value, _model, (m, v) => m.Cost = v, true);
    }

    [CustomValidation(typeof(ItemTypeViewModel), nameof(ValidateCategory))]
    public VanillaFlag Category
    {
        get => _model.Category;
        set => SetProperty(_model.Category, value, _model, (m, v) => m.Category = v, true);
    }

    public bool CountInMap
    {
        get => _model.Flags.CountInMap;
        set => SetProperty(_model.Flags.CountInMap, value, _model.Flags, (m, v) => m.CountInMap = v);
    }
    public bool CountInPlayer
    {
        get => _model.Flags.CountInPlayer;
        set => SetProperty(_model.Flags.CountInPlayer, value, _model.Flags, (m, v) => m.CountInPlayer = v);
    }
    public bool CountInCargo
    {
        get => _model.Flags.CountInCargo;
        set => SetProperty(_model.Flags.CountInCargo, value, _model.Flags, (m, v) => m.CountInCargo = v);
    }
    public bool CountInHoarder
    {
        get => _model.Flags.CountInHoarder;
        set => SetProperty(_model.Flags.CountInHoarder, value, _model.Flags, (m, v) => m.CountInHoarder = v);
    }
    public bool Crafted
    {
        get => _model.Flags.Crafted;
        set => SetProperty(_model.Flags.Crafted, value, _model.Flags, (m, v) => m.Crafted = v);
    }
    public bool Deloot
    {
        get => _model.Flags.Deloot;
        set => SetProperty(_model.Flags.Deloot, value, _model.Flags, (m, v) => m.Deloot = v);
    }

    [CustomValidation(typeof(ItemTypeViewModel), nameof(ValidateUsages))]
    public ObservableCollection<UserDefinableFlag> Usages { get => _model.Usages; }

    [CustomValidation(typeof(ItemTypeViewModel), nameof(ValidateValues))]
    public ObservableCollection<UserDefinableFlag> Value { get => _model.Value; }

    [CustomValidation(typeof(ItemTypeViewModel), nameof(ValidateTags))]
    public ObservableCollection<VanillaFlag> Tags { get => _model.Tags; }

    public IRelayCommand AddUsageFlagCommand { get; }
    public IRelayCommand RemoveUsageFlagCommand { get; }
    public IRelayCommand AddValueFlagCommand { get; }
    public IRelayCommand RemoveValueFlagCommand { get; }
    public IRelayCommand AddTagCommand { get; }
    public IRelayCommand RemoveTagCommand { get; }
    public IRelayCommand ClearFlagsCommand { get; }

    public ItemTypeViewModel(ItemType model, WorkspaceViewModel workspace)
        : base(new Dictionary<object, object>() { { "workspace", workspace } })
    {
        _dispatcher = Ioc.Default.GetRequiredService<IDispatcherService>();
        _model = model;

        AddUsageFlagCommand = new RelayCommand<UserDefinableFlag>(AddUsageFlag);
        RemoveUsageFlagCommand = new RelayCommand<UserDefinableFlag>(RemoveUsageFlag);
        AddValueFlagCommand = new RelayCommand<UserDefinableFlag>(AddValueFlag);
        RemoveValueFlagCommand = new RelayCommand<UserDefinableFlag>(RemoveValueFlag);
        AddTagCommand = new RelayCommand<VanillaFlag>(AddTag);
        RemoveTagCommand = new RelayCommand<VanillaFlag>(RemoveTag);
        ClearFlagsCommand = new RelayCommand<ClearTarget>(ClearFlags);
    }

    public void ValidateSelf() => ValidateAllProperties();
    public void AdjustQuantity(float factor)
    {
        Nominal = (int)Math.Round(Nominal * factor);
        Min = (int)Math.Round(Min * factor);
    }
    public void AdjustLifetime(float factor)
    {
        Lifetime = (int)Math.Round(Lifetime * factor);
    }
    public void AdjustRestock(float factor)
    {
        Restock = (int)Math.Round(Restock * factor);
    }
    protected void AddUsageFlag(UserDefinableFlag flag)
    {
        _dispatcher.BeginInvoke(() => Usages.Add(flag));
    }
    protected void RemoveUsageFlag(UserDefinableFlag flag)
    {
        Usages.Remove(flag);
    }
    protected void AddValueFlag(UserDefinableFlag flag)
    {
        _dispatcher.BeginInvoke(() => Value.Add(flag));
    }
    protected void RemoveValueFlag(UserDefinableFlag flag)
    {
        Value.Remove(flag);
    }
    protected void AddTag(VanillaFlag tag)
    {
        _dispatcher.BeginInvoke(() => Tags.Add(tag));
    }
    protected void RemoveTag(VanillaFlag tag)
    {
        Tags.Remove(tag);
    }
    protected void ClearFlags(ClearTarget target)
    {
        switch (target)
        {
            case ClearTarget.ValueFlags:
                _dispatcher.BeginInvoke(() => Value.Clear());
                break;
            case ClearTarget.UsageFlags:
                _dispatcher.BeginInvoke(() => Usages.Clear());
                break;
            case ClearTarget.Tags:
                _dispatcher.BeginInvoke(() => Tags.Clear());
                break;
            default: break;
        }
    }

    public static ValidationResult ValidateCategory(VanillaFlag category, ValidationContext context)
    {
        var workspace = (WorkspaceViewModel)context.Items["workspace"];
        var validator = new VanillaFalgValidator(workspace.Categories);

        var result = validator.Validate(category);
        return result == ValidationResult.Success ? result : new ValidationResult($"Category: {result.ErrorMessage}");
    }
    public static ValidationResult ValidateUsages(ObservableCollection<UserDefinableFlag> usages, ValidationContext context)
    {
        var workspace = (WorkspaceViewModel)context.Items["workspace"];
        var validator = new UserFlagValidator(workspace.Usages);

        if (usages.Where(usage => usage.DefinitionType == FlagDefinition.User).Count() > 1)
        {
            return new ValidationResult($"Usage Flags:\n\tOnly multiple user flags not allowed");
        }

        if (usages.Any(usage => usage.DefinitionType == FlagDefinition.User) &&
            usages.Any(usage => usage.DefinitionType == FlagDefinition.Vanilla))
        {
            return new ValidationResult($"Usage Flags:\n\tMixed flag definitions not allowed");
        }

        var errorsBuilder = new StringBuilder();
        ValidationResult itemResult;
        foreach (var usage in usages)
        {
            itemResult = validator.Validate(usage);
            if (itemResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\t{usage.ToString()}: {itemResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Usage Flags:\n{errors}");
        }
    }
    public static ValidationResult ValidateValues(ObservableCollection<UserDefinableFlag> values, ValidationContext context)
    {
        var workspace = (WorkspaceViewModel)context.Items["workspace"];
        var validator = new UserFlagValidator(workspace.Values);

        if (values.Where(value => value.DefinitionType == FlagDefinition.User).Count() > 1)
        {
            return new ValidationResult($"Value Flags:\n\tOnly multiple user flags not allowed");
        }

        if (values.Any(value => value.DefinitionType == FlagDefinition.User) &&
            values.Any(value => value.DefinitionType == FlagDefinition.Vanilla))
        {
            return new ValidationResult($"Value Flags:\n\tMixed flag definitions not allowed");
        }

        var errorsBuilder = new StringBuilder();
        ValidationResult itemResult;
        foreach (var value in values)
        {
            itemResult = validator.Validate(value);
            if (itemResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\t{value.ToString()}: {itemResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Value Flags:\n{errors}");
        }
    }
    public static ValidationResult ValidateTags(ObservableCollection<VanillaFlag> tags, ValidationContext context)
    {
        var workspace = (WorkspaceViewModel)context.Items["workspace"];
        var validator = new VanillaFalgValidator(workspace.Tags);

        var errorsBuilder = new StringBuilder();
        ValidationResult tagResult;
        foreach (var tag in tags)
        {
            tagResult = validator.Validate(tag);
            if (tagResult != ValidationResult.Success)
            {
                errorsBuilder.AppendLine($"\tTag {tag.Value}: {tagResult.ErrorMessage}");
            }
        }
        var errors = errorsBuilder.ToString();

        if (string.IsNullOrEmpty(errors))
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult($"Tags:\n{errors}");
        }
    }
}
