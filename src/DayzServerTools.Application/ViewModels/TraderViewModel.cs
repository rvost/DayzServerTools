using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.ViewModels;

public partial class TraderViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name), nameof(Categories))]
    private Trader model = new();

    public string Name
    {
        get => model.TraderName;
        set => SetProperty(model.TraderName, value, model, (m, n) => m.TraderName = n);
    }
    public ObservableCollection<TraderCategoryViewModel> Categories { get; }

    public IRelayCommand<string> AddCategoryCommand { get; }
    public IRelayCommand<TraderCategoryViewModel> RemoveCategoryCommand { get; }

    public TraderViewModel(Trader trader)
    {
        Model = trader;
        Categories = new ObservableCollection<TraderCategoryViewModel>(
            trader.TraderCategories.Select(c => new TraderCategoryViewModel(c))
            );

        AddCategoryCommand = new RelayCommand<string>(AddCategory);
        RemoveCategoryCommand = new RelayCommand<TraderCategoryViewModel>(RemoveCategory);

        Categories.CollectionChanged += CategoriesCollectionChanged;
    }

    private void CategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems)
                {
                    Model.TraderCategories.Add(((TraderCategoryViewModel)item).Model);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in e.OldItems)
                {
                    Model.TraderCategories.Remove(((TraderCategoryViewModel)item).Model);
                }
                break;
            default:
                break;
        }
    }

    public void AddCategory(string categoryName)
        => Categories.Add(new(new TraderCategory() { CategoryName = categoryName }));

    public void RemoveCategory(TraderCategoryViewModel category)
        => Categories.Remove(category);

    public void Dispose()
    {
        Categories.CollectionChanged -= CategoriesCollectionChanged;
    }
}
