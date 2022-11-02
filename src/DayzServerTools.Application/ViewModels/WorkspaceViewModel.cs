using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.ItemTypes;
using DayzServerTools.Application.ViewModels.Panes;
using DayzServerTools.Application.ViewModels.RandomPresets;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Application.ViewModels.UserDefinitions;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Xml;

using RandomPresetsModel = DayzServerTools.Library.Xml.RandomPresets;
using UserDefinitionsModel = DayzServerTools.Library.Xml.UserDefinitions;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Library.Common;

namespace DayzServerTools.Application.ViewModels;

public enum NewTabOptions
{
    NewTypes,
    OpenTypes,
    NewUserDefinitions,
    OpenUserDefinitions,
    NewRandomPresets,
    OpenRandomPresets,
    NewSpawnableTypes,
    OpenSpawnableTypes,
    OpenTraderConfig
}

public partial class WorkspaceViewModel : TabbedViewModel, ILimitsDefinitionsProvider
{
    private readonly IDialogFactory _dialogFactory;
    private readonly ErrorsPaneViewModel _errorsPaneViewModel;

    private LimitsDefinitions limitsDefinitions = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UserDefinitionsLoaded))]
    [NotifyCanExecuteChangedFor(nameof(LoadUserDefinitionsCommand))]
    private UserDefinitionsModel userDefinitions = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RandomPresetsLoaded))]
    [NotifyCanExecuteChangedFor(nameof(LoadRandomPresetsCommand))]
    private RandomPresetsModel randomPresets = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActiveFileIsUserDefinitions), nameof(ActiveFileIsItemTypes), 
        nameof(ActiveFileIsRandomPresets), nameof(ActiveFileIsSpawnableTypes), 
        nameof(ActiveFileIsTraderConfig))]
    private IProjectFileTab activeFile;
    private object activePane;

    public object ActivePane
    {
        get => activePane;
        set
        {
            SetProperty(ref activePane, value);
            if(value is IProjectFileTab)
            {
                ActiveFile = (IProjectFileTab)value;
            }
        }
    }
    public bool ActiveFileIsUserDefinitions => ActiveFile is UserDefinitionsViewModel;
    public bool ActiveFileIsItemTypes => ActiveFile is ItemTypesViewModel;
    public bool ActiveFileIsRandomPresets => ActiveFile is RandomPresetsViewModel;
    public bool ActiveFileIsSpawnableTypes => ActiveFile is SpawnableTypesViewModel;
    public bool ActiveFileIsTraderConfig => ActiveFile is TraderConfigViewModel;
    public ErrorsPaneViewModel ErrorsPaneViewModel => _errorsPaneViewModel;
    [ObservableProperty]
    private ObservableCollection<UserDefinableFlag> usages = new();
    [ObservableProperty]
    private ObservableCollection<UserDefinableFlag> values = new();
    [ObservableProperty]
    private ObservableCollection<VanillaFlag> categories = new() { new VanillaFlag("") };
    [ObservableProperty]
    private ObservableCollection<VanillaFlag> tags = new();
    [ObservableProperty]
    private IEnumerable<string> availableCargoPresets = new List<string>();
    [ObservableProperty]
    private IEnumerable<string> availableAttachmentsPresets = new List<string>();

    public LimitsDefinitions LimitsDefinitions
    {
        get => limitsDefinitions;
        set
        {
            var oldValue = limitsDefinitions;
            if(SetProperty(ref limitsDefinitions, value))
            {
                OnPropertyChanged(nameof(LimitsDefinitionsLoaded));
                LoadLimitsDefinitionsCommand.NotifyCanExecuteChanged();
                WeakReferenceMessenger.Default.Send(
                    new LimitsDefinitionsChengedMaessage(this, nameof(LimitsDefinitions), oldValue, value)
                    );
            }
        }
    }
    public bool LimitsDefinitionsLoaded { get => LimitsDefinitions is not null; }
    public bool UserDefinitionsLoaded { get => UserDefinitions is not null; }
    public bool RandomPresetsLoaded { get => RandomPresets is not null; }
    public IEnumerable<IPane> Panes { get; }

    public IRelayCommand LoadLimitsDefinitionsCommand { get; }
    public IRelayCommand LoadUserDefinitionsCommand { get; }
    public IRelayCommand LoadRandomPresetsCommand { get; }
    public IRelayCommand<NewTabOptions> NewTabCommand { get; }
    public IRelayCommand SaveAllCommand { get; }

    public WorkspaceViewModel(IDialogFactory dialogFactory, ErrorsPaneViewModel errorsPaneViewModel) : base()
    {
        _dialogFactory = dialogFactory;
        
        _errorsPaneViewModel = errorsPaneViewModel;
        Panes = new List<IPane>() { _errorsPaneViewModel };

        LoadLimitsDefinitionsCommand = new RelayCommand(LoadLimitsDefinitions, () => LimitsDefinitions is null);
        LoadUserDefinitionsCommand = new RelayCommand(LoadUserDefinitions, () => UserDefinitions is null);
        LoadRandomPresetsCommand = new RelayCommand(LoadRandomPresets, () => RandomPresets is null);
        NewTabCommand = new RelayCommand<NewTabOptions>(NewTab);
        SaveAllCommand = new RelayCommand(SaveAll, () => Tabs.Count > 0);
    }

    public void LoadLimitsDefinitions()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfglimitsdefinition.*";
        dialog.ShowDialog();
        var filename = dialog.FileName;

        if (File.Exists(filename))
        {
            using var input = File.OpenRead(filename);
            try
            {
                LimitsDefinitions = LimitsDefinitions.ReadFromStream(input);
                Categories.AddRange(LimitsDefinitions.Categories);
                Usages.AddRange(LimitsDefinitions.UsageFlags);
                Values.AddRange(LimitsDefinitions.ValueFlags);
                Tags.AddRange(LimitsDefinitions.Tags);
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = _dialogFactory.CreateMessageDialog();
                errorDialog.Title = "File format error";
                errorDialog.Message = e.InnerException.Message;
                errorDialog.Image = MessageDialogImage.Error;
                errorDialog.Show();
            }
        }
    }
    public void LoadUserDefinitions()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfglimitsdefinitionuser*";
        dialog.ShowDialog();
        var filename = dialog.FileName;

        if (File.Exists(filename))
        {
            using var input = File.OpenRead(filename);
            try
            {
                UserDefinitions = UserDefinitionsModel.ReadFromStream(input);
                Usages.AddRange(UserDefinitions.UsageFlags.Select(def => (UserDefinableFlag)def));
                Values.AddRange(UserDefinitions.ValueFlags.Select(def => (UserDefinableFlag)def));
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = _dialogFactory.CreateMessageDialog();
                errorDialog.Title = "File format error";
                errorDialog.Message = e.InnerException.Message;
                errorDialog.Image = MessageDialogImage.Error;
                errorDialog.Show();
            }
        }
    }
    public void LoadRandomPresets()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = "cfgrandompresets*";
        dialog.ShowDialog();
        var filename = dialog.FileName;

        if (File.Exists(filename))
        {
            using var input = File.OpenRead(filename);
            try
            {
                RandomPresets = RandomPresetsModel.ReadFromStream(input);
                AvailableCargoPresets = RandomPresets.CargoPresets.Select(p => p.Name).ToList();
                AvailableAttachmentsPresets = RandomPresets.AttachmentsPresets.Select(p => p.Name).ToList();
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = _dialogFactory.CreateMessageDialog();
                errorDialog.Title = "File format error";
                errorDialog.Message = e.InnerException.Message;
                errorDialog.Image = MessageDialogImage.Error;
                errorDialog.Show();
            }
        }
    }
    public void NewTab(NewTabOptions options)
    {
        switch (options)
        {
            case NewTabOptions.NewTypes:
                CreateItemTypes();
                break;
            case NewTabOptions.OpenTypes:
                OpenItemTypes();
                break;
            case NewTabOptions.NewUserDefinitions:
                CreateUserDefinitions();
                break;
            case NewTabOptions.OpenUserDefinitions:
                OpenUserDefinitions();
                break;
            case NewTabOptions.OpenTraderConfig:
                OpenTraderConfig();
                break;
            case NewTabOptions.NewRandomPresets:
                CreateRandomPresets();
                break;
            case NewTabOptions.OpenRandomPresets:
                OpenRandomPresets();
                break;
            case NewTabOptions.NewSpawnableTypes:
                CreateSpawnableTypes();
                break;
            case NewTabOptions.OpenSpawnableTypes:
                OpenSpawnableTypes();
                break;
            default:
                OpenItemTypes();
                break;
        }
    }
    public void SaveAll()
        => Tabs.AsParallel().ForAll(tab => tab.SaveCommand.Execute(null));
    public void CreateItemTypes()
    {
        var newItemTypesVM = Ioc.Default.GetService<ItemTypesViewModel>();
        Tabs.Add(newItemTypesVM);
    }
    public void CreateItemTypes(IEnumerable<ItemType> items)
    {
        var newItemTypesVM = Ioc.Default.GetService<ItemTypesViewModel>();
        newItemTypesVM.CopyItemTypes(items);
        Tabs.Add(newItemTypesVM);
    }
    public void OpenItemTypes()
    {
        var newItemTypesVM = Ioc.Default.GetService<ItemTypesViewModel>();
        Tabs.Add(newItemTypesVM);
        newItemTypesVM.Load();
    }
    public void CreateUserDefinitions()
    {
        var newUserDefinitionVM = Ioc.Default.GetService<UserDefinitionsViewModel>();
        newUserDefinitionVM.LimitsDefinitions = LimitsDefinitions;
        Tabs.Add(newUserDefinitionVM);
    }
    public void OpenUserDefinitions()
    {
        var newUserDefinitionVM = Ioc.Default.GetService<UserDefinitionsViewModel>();
        newUserDefinitionVM.LimitsDefinitions = LimitsDefinitions;
        Tabs.Add(newUserDefinitionVM);
        newUserDefinitionVM.Load();
    }
    public void CreateRandomPresets()
    {
        var newVM= Ioc.Default.GetService<RandomPresetsViewModel>();
        Tabs.Add(newVM);
    }
    public void OpenRandomPresets()
    {
        var newVM = Ioc.Default.GetService<RandomPresetsViewModel>();
        Tabs.Add(newVM);
        newVM.Load();
    }
    public void CreateSpawnableTypes()
    {
        var newVM = Ioc.Default.GetService<SpawnableTypesViewModel>();
        newVM.Workspace = this;
        Tabs.Add(newVM);
    }
    public void CreateSpawnableTypes(IEnumerable<SpawnableType> items)
    {
        var newVM = Ioc.Default.GetService<SpawnableTypesViewModel>();
        newVM.Workspace = this;
        newVM.CopySpawnableTypes(items);
        Tabs.Add(newVM);
    }
    public void OpenSpawnableTypes()
    {
        var newVM = Ioc.Default.GetService<SpawnableTypesViewModel>();
        newVM.Workspace = this;
        Tabs.Add(newVM);
        newVM.Load();
    }
    public void OpenTraderConfig()
    {
        var newVM = Ioc.Default.GetService<TraderConfigViewModel>();
        //newVM.Workspace = this;
        Tabs.Add(newVM);
        newVM.Load();
    }

    protected override void TabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        base.TabsCollectionChanged(sender, e);
        SaveAllCommand.NotifyCanExecuteChanged();
    }

    protected override void OnTabCloseRequested(object sender, EventArgs e)
    {
        if(sender == ActiveFile)
        {
            ActiveFile = null;
        }
        base.OnTabCloseRequested(sender, e);
    }
}
