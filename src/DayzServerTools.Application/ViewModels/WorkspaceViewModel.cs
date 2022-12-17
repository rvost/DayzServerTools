using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.ItemTypes;
using DayzServerTools.Application.ViewModels.Panes;
using DayzServerTools.Application.ViewModels.RandomPresets;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Application.ViewModels.UserDefinitions;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Application.Extensions;
using DayzServerTools.Application.Messages;
using DayzServerTools.Library.Common;
using DayzServerTools.Library.Xml;
using DayzServerTools.Library.Trader;

using ItemTypesModel = DayzServerTools.Library.Xml.ItemTypes;
using RandomPresetsModel = DayzServerTools.Library.Xml.RandomPresets;
using SpawnableTypesModel = DayzServerTools.Library.Xml.SpawnableTypes;
using UserDefinitionsModel = DayzServerTools.Library.Xml.UserDefinitions;

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

public partial class WorkspaceViewModel : TabbedViewModel, ILimitsDefinitionsProvider, IRandomPresetsProvider
{
    private readonly ProjectFileViewModelFactory _fileViewModelFactory;
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
            if (value is IProjectFileTab)
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
            if (SetProperty(ref limitsDefinitions, value))
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

    public WorkspaceViewModel(IDialogFactory dialogFactory, ErrorsPaneViewModel errorsPaneViewModel,
        ProjectFileViewModelFactory fileViewModelFactory) : base()
    {
        _fileViewModelFactory = fileViewModelFactory;
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
          IProjectFileTab tab = options switch
        {
            NewTabOptions.NewTypes => _fileViewModelFactory.Create<ItemTypesModel>("types.xml", null),
            NewTabOptions.OpenTypes => OpenTabWithDialog<ItemTypesModel>(OpenFileDialogOptions.TypesOptions),

            NewTabOptions.NewUserDefinitions => _fileViewModelFactory.Create<UserDefinitionsModel>("cfglimitsdefinitionsuser.xml", null),
            NewTabOptions.OpenUserDefinitions => OpenTabWithDialog<UserDefinitionsModel>(OpenFileDialogOptions.UserDefinitionsOptions),

            NewTabOptions.NewRandomPresets => _fileViewModelFactory.Create<RandomPresetsModel>("cfgrandompresets.xml", null),
            NewTabOptions.OpenRandomPresets => OpenTabWithDialog<RandomPresetsModel>(OpenFileDialogOptions.RandomPresetsOptions),

            NewTabOptions.NewSpawnableTypes => _fileViewModelFactory.Create<SpawnableTypesModel>("cfgspawnabletypes.xml", null),
            NewTabOptions.OpenSpawnableTypes => OpenTabWithDialog<SpawnableTypesModel>(OpenFileDialogOptions.SpawnableTypesOptions),

            NewTabOptions.OpenTraderConfig => OpenTraderConfig(),
        };

        if (tab is not null)
        {
            Tabs.Add(tab);
        }
    }
    public void SaveAll()
        => Tabs.AsParallel().ForAll(tab => tab.SaveCommand.Execute(null));

    public void CreateItemTypes(IEnumerable<ItemType> items)
    {
        var newItemTypesVM = (ItemTypesViewModel)_fileViewModelFactory.Create<ItemTypesModel>("types.xml", null);
        newItemTypesVM.CopyItemTypes(items);
        Tabs.Add(newItemTypesVM);
    }
   
    public void CreateSpawnableTypes(IEnumerable<SpawnableType> items)
    {
        var newVM = (SpawnableTypesViewModel)_fileViewModelFactory.Create<SpawnableTypesModel>("cfgspawnabletypes.xml", null);
        newVM.CopySpawnableTypes(items);
        Tabs.Add(newVM);
    }
   
    public IProjectFileTab OpenTraderConfig()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();

        dialog.FileName = OpenFileDialogOptions.TraderConfigOptions.FileName;
        dialog.Filter = OpenFileDialogOptions.TraderConfigOptions.Filter;

        dialog.ShowDialog();
        var filename = dialog.FileName;

        if (File.Exists(filename))
        {
            using var input = File.OpenRead(filename);
            try
            {
                var model = TraderConfig.ReadFromStream(input);
                return _fileViewModelFactory.Create(filename, model);

            }
            catch (InvalidOperationException e)
            {
                var errorDialog = _dialogFactory.CreateMessageDialog();
                errorDialog.Title = "File format error";
                errorDialog.Message = e.InnerException?.Message ?? e.Message;
                errorDialog.Image = MessageDialogImage.Error;
                errorDialog.Show();
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private IProjectFileTab OpenTabWithDialog<T>(OpenFileDialogOptions options) where T : IProjectFile, new()
    {
        var dialog = _dialogFactory.CreateOpenFileDialog();
        dialog.FileName = options.FileName;
        dialog.Filter = options.Filter;

        dialog.ShowDialog();
        var filename = dialog.FileName;

        if (File.Exists(filename))
        {
            using var input = File.OpenRead(filename);
            try
            {
                var model = DayzXmlFile<T>.ReadFromStream(input);
                return _fileViewModelFactory.Create<T>(filename, model);

            }
            catch (InvalidOperationException e)
            {
                var errorDialog = _dialogFactory.CreateMessageDialog();
                errorDialog.Title = "File format error";
                errorDialog.Message = e.InnerException?.Message ?? e.Message;
                errorDialog.Image = MessageDialogImage.Error;
                errorDialog.Show();
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    protected override void TabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        base.TabsCollectionChanged(sender, e);
        SaveAllCommand.NotifyCanExecuteChanged();
    }

    protected override void OnTabCloseRequested(object sender, EventArgs e)
    {
        if (sender == ActiveFile)
        {
            ActiveFile = null;
        }
        base.OnTabCloseRequested(sender, e);
    }
}
