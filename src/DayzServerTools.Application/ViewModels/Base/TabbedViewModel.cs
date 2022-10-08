using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DayzServerTools.Application.ViewModels.Base;

public abstract partial class TabbedViewModel : ObservableObject
{
    [ObservableProperty]
    protected ObservableCollection<IProjectFileTab> tabs = new();

    public TabbedViewModel()
    {
        Tabs.CollectionChanged += TabsCollectionChanged;
    }

    protected virtual void TabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        IProjectFileTab tab;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                tab = (IProjectFileTab)e.NewItems[0];
                tab.CloseRequested += OnTabCloseRequested;
                break;
            case NotifyCollectionChangedAction.Remove:
                tab = (IProjectFileTab)e.OldItems[0];
                tab.CloseRequested -= OnTabCloseRequested;
                break;
            default:
                break;
        }
    }

    protected virtual void OnTabCloseRequested(object sender, EventArgs e)
    {
        Tabs.Remove((IProjectFileTab)sender);
        if(sender is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
