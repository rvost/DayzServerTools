using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;
using Fluent;

using DayzServerTools.Application.ViewModels;


namespace DayzServerTools.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<WorkspaceViewModel>();
        }
    }
}
