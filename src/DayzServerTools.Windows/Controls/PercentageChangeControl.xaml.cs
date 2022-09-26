using System;
using System.Collections.Generic;
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
    /// Interaction logic for PercentageChangeControl.xaml
    /// </summary>
    public partial class PercentageChangeControl : UserControl
    {

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public float Percentage
        {
            get { return (float)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }
        public float MinValue
        {
            get { return (float)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public float MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
       public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(float), typeof(PercentageChangeControl), new PropertyMetadata(default(float)));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(PercentageChangeControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(float), typeof(PercentageChangeControl), new PropertyMetadata(default(float)));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(float), typeof(PercentageChangeControl), new PropertyMetadata(default(float)));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PercentageChangeControl), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(PercentageChangeControl), new PropertyMetadata(null));

        public PercentageChangeControl()
        {
            InitializeComponent();
        }
    }
}
