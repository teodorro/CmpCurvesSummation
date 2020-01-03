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
    /// Interaction logic for StepsCtrl.xaml
    /// </summary>
    public partial class StepsCtrl : UserControl
    {
        private StepsViewModel _viewModel;
        public StepsViewModel ViewModel => _viewModel;

        public StepsCtrl()
        {
            InitializeComponent();

            _viewModel = new StepsViewModel();
            DataContext = _viewModel;
        }
    }
}
