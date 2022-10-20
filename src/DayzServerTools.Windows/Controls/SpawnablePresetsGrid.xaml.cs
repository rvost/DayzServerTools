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
    /// Interaction logic for SpawnablePresetsGrid.xaml
    /// </summary>
    public partial class SpawnablePresetsGrid : UserControl
    {
        public ObservableCollection<SpawnablePresetViewModel> ItemsSource
        {
            get { return (ObservableCollection<SpawnablePresetViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public IEnumerable<string> AvailablePresets
        {
            get { return (IEnumerable<string>)GetValue(AvailablePresetsProperty); }
            set { SetValue(AvailablePresetsProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<SpawnablePresetViewModel>), typeof(SpawnablePresetsGrid), new PropertyMetadata(default));
        
        public static readonly DependencyProperty AvailablePresetsProperty =
           DependencyProperty.Register("AvailablePresets", typeof(IEnumerable<string>), typeof(SpawnablePresetsGrid), new PropertyMetadata(default));
        
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SpawnablePresetsGrid), new PropertyMetadata(default));

        public SpawnablePresetsGrid()
        {
            InitializeComponent();
        }
    }
}
