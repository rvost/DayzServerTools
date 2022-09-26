using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using DayzServerTools.Application.ViewModels;

namespace DayzServerTools.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<WorkspaceViewModel>();
        }
    }
}
