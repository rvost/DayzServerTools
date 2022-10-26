using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
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

    [CustomValidation(typeof(ItemTypesValidation), nameof(ItemTypesValidation.ValidateCategory))]
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

    [CustomValidation(typeof(ItemTypesValidation), nameof(ItemTypesValidation.ValidateUsages))]
    public ObservableCollection<UserDefinableFlag> Usages { get => _model.Usages; }

    [CustomValidation(typeof(ItemTypesValidation), nameof(ItemTypesValidation.ValidateValues))]
    public ObservableCollection<UserDefinableFlag> Value { get => _model.Value; }

    [CustomValidation(typeof(ItemTypesValidation), nameof(ItemTypesValidation.ValidateTags))]
    public ObservableCollection<VanillaFlag> Tags { get => _model.Tags; }

    public IRelayCommand AddUsageFlagCommand { get; }
    public IRelayCommand RemoveUsageFlagCommand { get; }
    public IRelayCommand AddValueFlagCommand { get; }
    public IRelayCommand RemoveValueFlagCommand { get; }
    public IRelayCommand AddTagCommand { get; }
    public IRelayCommand RemoveTagCommand { get; }
    public IRelayCommand ClearFlagsCommand { get; }

    public ItemTypeViewModel(ItemType model, WorkspaceViewModel workspace)
        : base(new Dictionary<object, object>() {
            { "categories", () => workspace.Categories },
            { "usages", () => workspace.Usages },
            { "values", () => workspace.Values },
            { "tags", () => workspace.Tags }
        })
    {
        _dispatcher = Ioc.Default.GetRequiredService<IDispatcherService>();
        _model = model;
        _model.Value.RemoveAllEmpty();
        _model.Usages.RemoveAllEmpty();
        _model.Tags.RemoveAllEmpty();

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
}
