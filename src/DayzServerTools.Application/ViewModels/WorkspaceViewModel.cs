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
using DayzServerTools.Library.Missions;
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
    OpenUserDefinitions,
    OpenRandomPresets,
    NewSpawnableTypes,
    OpenSpawnableTypes,
    OpenTraderConfig
}

public partial class WorkspaceViewModel : TabbedViewModel, ILimitsDefinitionsProvider, IRandomPresetsProvider,
    IRecipient<IProjectFileTab>
{
    private readonly ProjectFileViewModelFactory _fileViewModelFactory;
    private readonly IDialogFactory _dialogFactory;
    private readonly ErrorsPaneViewModel _errorsPaneViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MissionLoaded))]
    [NotifyCanExecuteChangedFor(nameof(LoadMissionCommand), nameof(EditUserDefinitionsCommand),
        nameof(EditRandomPresetsCommand), nameof(EditTypesCommand), nameof(EditSpawnableTypesCommand))]
    private MpMission _mission;

    private LimitsDefinitions limitsDefinitions = null;
    [ObservableProperty]
    private UserDefinitionsModel userDefinitions = null;
    [ObservableProperty]
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
                WeakReferenceMessenger.Default.Send(
                    new LimitsDefinitionsChengedMaessage(this, nameof(LimitsDefinitions), oldValue, value)
                    );
            }
        }
    }

    public bool MissionLoaded { get => Mission is not null; }

    public IEnumerable<IPane> Panes { get; }

    public IAsyncRelayCommand LoadMissionCommand { get; }
    public IRelayCommand EditUserDefinitionsCommand { get; }
    public IRelayCommand EditRandomPresetsCommand { get; }
    public IRelayCommand EditTypesCommand { get; }
    public IRelayCommand EditSpawnableTypesCommand { get; }
    public IRelayCommand<NewTabOptions> NewTabCommand { get; }
    public IRelayCommand SaveAllCommand { get; }

    public WorkspaceViewModel(IDialogFactory dialogFactory, ErrorsPaneViewModel errorsPaneViewModel,
        ProjectFileViewModelFactory fileViewModelFactory) : base()
    {
        _fileViewModelFactory = fileViewModelFactory;
        _dialogFactory = dialogFactory;

        _errorsPaneViewModel = errorsPaneViewModel;
        Panes = new List<IPane>() { _errorsPaneViewModel };

        LoadMissionCommand = new AsyncRelayCommand(LoadMission, () => Mission is null);
        EditUserDefinitionsCommand = new RelayCommand(EditUserDefinitions, () => Mission is not null);
        EditRandomPresetsCommand = new RelayCommand(EditRandomPresets, () => Mission is not null);
        EditTypesCommand = new RelayCommand(EditTypes, () => Mission is not null);
        EditSpawnableTypesCommand = new RelayCommand(EditSpawnableTypes, () => Mission is not null);

        NewTabCommand = new RelayCommand<NewTabOptions>(NewTab);
        SaveAllCommand = new RelayCommand(SaveAll, () => Tabs.Count > 0);
        // TODO: Inject Messenger
        WeakReferenceMessenger.Default.Register(this);
    }

    public async Task LoadMission()
    {
        var dialog = _dialogFactory.CreateOpenFolderDialog();
        dialog.Title = "Select Mission Folder";
        dialog.ShowDialog();
        var folder = dialog.FileName;
        try
        {
            Mission = await MpMission.Open(folder);
            // TODO: Refactor
            LimitsDefinitions = Mission.LimitsDefinitions;
            Categories.AddRange(LimitsDefinitions.Categories);
            Usages.AddRange(LimitsDefinitions.UsageFlags);
            Values.AddRange(LimitsDefinitions.ValueFlags);
            Tags.AddRange(LimitsDefinitions.Tags);

            UserDefinitions = Mission.UserDefinitions;
            Usages.AddRange(UserDefinitions.UsageFlags.Select(def => (UserDefinableFlag)def));
            Values.AddRange(UserDefinitions.ValueFlags.Select(def => (UserDefinableFlag)def));

            RandomPresets = Mission.RandomPresets;
            AvailableCargoPresets = RandomPresets.CargoPresets.Select(p => p.Name).ToList();
            AvailableAttachmentsPresets = RandomPresets.AttachmentsPresets.Select(p => p.Name).ToList();
        }
        catch (Exception ex)
        {
            var errorDialog = _dialogFactory.CreateMessageDialog();
            errorDialog.Title = "Mission file error";
            errorDialog.Message = ex.Message;
            errorDialog.Image = MessageDialogImage.Error;
            errorDialog.Show();
        }
    }
    public void EditUserDefinitions()
    {
        var fullPath = Path.Combine(Mission.MissionFolder, MissionFiles.UserDefinitions);
        var tab = _fileViewModelFactory.Create(fullPath, Mission.UserDefinitions);
        Tabs.Add(tab);
    }
    public void EditRandomPresets()
    {
        var fullPath = Path.Combine(Mission.MissionFolder, MissionFiles.RandomPresets);
        var tab = _fileViewModelFactory.Create(fullPath, Mission.RandomPresets);
        Tabs.Add(tab);
    }
    public void EditTypes()
    {
        var fullPath = Path.Combine(Mission.MissionFolder, MissionFiles.Types);
        try
        {
            var model = ItemTypesModel.ReadFromFile(fullPath);
            var tab = _fileViewModelFactory.Create(fullPath, model);
            Tabs.Add(tab);
        }
        catch (Exception e)
        {
            var errorDialog = _dialogFactory.CreateMessageDialog();
            errorDialog.Title = "File error";
            errorDialog.Message = e.InnerException?.Message ?? e.Message;
            errorDialog.Image = MessageDialogImage.Error;
            errorDialog.Show();
        }
    }
    public void EditSpawnableTypes()
    {
        var fullPath = Path.Combine(Mission.MissionFolder, MissionFiles.SpawnableTypes);
        try
        {
            var model = SpawnableTypesModel.ReadFromFile(fullPath);
            var tab = _fileViewModelFactory.Create(fullPath, model);
            Tabs.Add(tab);
        }
        catch (Exception e)
        {
            var errorDialog = _dialogFactory.CreateMessageDialog();
            errorDialog.Title = "File error";
            errorDialog.Message = e.InnerException?.Message ?? e.Message;
            errorDialog.Image = MessageDialogImage.Error;
            errorDialog.Show();
        }
    }

    public void NewTab(NewTabOptions options)
    {
        IProjectFileTab tab = options switch
        {
            NewTabOptions.NewTypes => _fileViewModelFactory.Create<ItemTypesModel>("types.xml", null),
            NewTabOptions.OpenTypes => OpenTabWithDialog<ItemTypesModel>(OpenFileDialogOptions.TypesOptions),

            NewTabOptions.OpenUserDefinitions => OpenTabWithDialog<UserDefinitionsModel>(OpenFileDialogOptions.UserDefinitionsOptions),
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

    private IProjectFileTab OpenTraderConfig()
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

    public void Receive(IProjectFileTab tab)
    {
        if (tab != null && !Tabs.Contains(tab))
        {
            Tabs.Add(tab);
        }
    }
}
