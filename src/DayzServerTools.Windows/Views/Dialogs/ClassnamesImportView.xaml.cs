using DayzServerTools.Application.ViewModels;
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
    /// Interaction logic for ClassnamesImportView.xaml
    /// </summary>
    public partial class ClassnamesImportView : Window
    {
        private readonly ClassnamesImportViewModel _model;
        public ClassnamesImportView(ClassnamesImportViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = _model;
            _model.CloseRequested += OnCloseRequested;
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            DialogResult = true;
            _model.CloseRequested -= OnCloseRequested;
            Close();
        }
    }
}
