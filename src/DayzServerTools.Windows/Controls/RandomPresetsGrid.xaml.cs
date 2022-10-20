using DayzServerTools.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DayzServerTools.Windows.Controls
{
    /// <summary>
    /// Interaction logic for RandomPresetsGrid.xaml
    /// </summary>
    public partial class RandomPresetsGrid : UserControl
    {
        public ObservableCollection<RandomPresetViewModel> ItemsSource
        {
            get { return (ObservableCollection<RandomPresetViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<RandomPresetViewModel>), typeof(RandomPresetsGrid), new PropertyMetadata(default));
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(RandomPresetsGrid), new PropertyMetadata(default));

        public RandomPresetsGrid()
        {
            InitializeComponent();
        }
    }
}
