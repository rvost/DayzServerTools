using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayzServerTools.Library.Trader;

namespace DayzServerTools.Application.ViewModels
{
    public partial class TraderViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Categories), nameof(Name))]
        private Trader model;

        public string Name { 
            get => model.TraderName; 
            set => SetProperty(model.TraderName, value, model, (m, n) => m.TraderName = n);
        }
        public ObservableCollection<TraderCategory> Categories { get => model.TraderCategories; }
        public IRelayCommand<string> AddCategoryCommand { get; }
        public IRelayCommand<TraderCategory> RemoveCategoryCommand { get; }

        public TraderViewModel(Trader trader)
        {
            Model = trader;

            AddCategoryCommand = new RelayCommand<string>(AddCategory);
            RemoveCategoryCommand = new RelayCommand<TraderCategory>(RemoveCategory);
        }

        public void AddCategory(string categoryName)
            => Categories.Add(new TraderCategory() { CategoryName = categoryName });

        public void RemoveCategory(TraderCategory category)
            => Categories.Remove(category);
    }
}
