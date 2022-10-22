using DayzServerTools.Application.ViewModels.Dialogs;
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
using System.Windows.Shapes;

namespace DayzServerTools.Windows.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for SpawnableTypesExportView.xaml
    /// </summary>
    public partial class ExportView : Window
    {
        private IExportViewModel _model;

        public ExportView(IExportViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = model;
            model.CloseRequested += OnCloseRequested;
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            DialogResult = true;
            _model.CloseRequested -= OnCloseRequested;
            Close();
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _model.ExportCommand.NotifyCanExecuteChanged();
        }
    }
}
