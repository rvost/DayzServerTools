using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Library.Trader;
using DayzServerTools.Library.Trader.Validators;
using TraderModel = DayzServerTools.Library.Trader.Trader;

namespace DayzServerTools.Application.ViewModels.Trader;

public partial class TraderViewModel : ObservableFluentValidator<TraderModel, TraderValidator>, IDisposable
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveCategoryCommand))]
    private TraderCategoryViewModel selectedCategory;

    public string Name
    {
        get => _model.TraderName;
        set => SetProperty(_model.TraderName, value, _model, (m, n) => m.TraderName = n);
    }
    public ObservableCollection<TraderCategoryViewModel> Categories { get; }

    public IRelayCommand<string> AddCategoryCommand { get; }
    public IRelayCommand<TraderCategoryViewModel> RemoveCategoryCommand { get; }

    public TraderViewModel(TraderModel trader):base(trader, new())
    {
        Categories = new ObservableCollection<TraderCategoryViewModel>(
            trader.TraderCategories.Select(c => new TraderCategoryViewModel(c))
            );

        AddCategoryCommand = new RelayCommand<string>(AddCategory);
        RemoveCategoryCommand = new RelayCommand<TraderCategoryViewModel>(RemoveCategory, CanRemoveCategory);

        Categories.CollectionChanged += CategoriesCollectionChanged;
    }
    protected void AddCategory(string categoryName)
           => Categories.Add(new(new TraderCategory() { CategoryName = categoryName }));
    protected bool CanRemoveCategory(TraderCategoryViewModel category)
        => category is not null || SelectedCategory is not null;
    protected void RemoveCategory(TraderCategoryViewModel category)
    {
        if (category is not null)
        {
            Categories.Remove(category);
        }
        else if (SelectedCategory is not null)
        {
            Categories.Remove(SelectedCategory);
            SelectedCategory = null;
        }
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

    public void Dispose()
    {
        Categories.CollectionChanged -= CategoriesCollectionChanged;
    }
}
