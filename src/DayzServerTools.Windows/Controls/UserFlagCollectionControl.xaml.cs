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

using DayzServerTools.Library.Xml;

namespace DayzServerTools.Windows.Controls
{
    /// <summary>
    /// Interaction logic for UserFlagCollectionControl.xaml
    /// </summary>
    public partial class UserFlagCollectionControl : UserControl
    {

        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }
        public ICommand RemoveItemCommand
        {
            get { return (ICommand)GetValue(RemoveItemCommandProperty); }
            set { SetValue(RemoveItemCommandProperty, value); }
        }
        public ObservableCollection<UserDefinableFlag> Items
        {
            get { return (ObservableCollection<UserDefinableFlag>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public IEnumerable<UserDefinableFlag> NewItemOptions
        {
            get { return (IEnumerable<UserDefinableFlag>)GetValue(NewItemOptionsProperty); }
            set { SetValue(NewItemOptionsProperty, value); }
        }

        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(UserFlagCollectionControl), new PropertyMetadata(default));

        public static readonly DependencyProperty RemoveItemCommandProperty =
            DependencyProperty.Register("RemoveItemCommand", typeof(ICommand), typeof(UserFlagCollectionControl), new PropertyMetadata(default));

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<UserDefinableFlag>), typeof(UserFlagCollectionControl), new PropertyMetadata(default));

        public static readonly DependencyProperty NewItemOptionsProperty =
            DependencyProperty.Register("NewItemOptions", typeof(IEnumerable<UserDefinableFlag>), typeof(UserFlagCollectionControl), new PropertyMetadata(default));


        public bool ShowNewItemControl
        {
            get { return (bool)GetValue(ShowNewItemControlProperty); }
            set { SetValue(ShowNewItemControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNewItemControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNewItemControlProperty =
            DependencyProperty.Register("ShowNewItemControl", typeof(bool), typeof(UserFlagCollectionControl), new PropertyMetadata(default));


        public UserFlagCollectionControl()
        {
            InitializeComponent();
        }
    }
}
