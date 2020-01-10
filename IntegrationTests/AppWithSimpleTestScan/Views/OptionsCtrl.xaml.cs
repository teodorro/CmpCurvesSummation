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
using AppWithSimpleTestScan.ViewModels;

namespace AppWithSimpleTestScan.Views
{
    /// <summary>
    /// Interaction logic for OptionsCtrl.xaml
    /// </summary>
    public partial class OptionsCtrl : UserControl
    {
        private OptionsViewModel _viewModel;
        public OptionsViewModel ViewModel => _viewModel;

        public OptionsCtrl()
        {
            InitializeComponent();

            _viewModel = new OptionsViewModel();
            DataContext = _viewModel;
        }

        private void ButtonSummation_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchSummation();
        }
    }
}
