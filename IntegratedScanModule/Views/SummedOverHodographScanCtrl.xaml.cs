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
using IntegratedScanModule.ViewModels;

namespace IntegratedScanModule.Views
{
    /// <summary>
    /// Interaction logic for SummedOverHodographScanCtrl.xaml
    /// </summary>
    public partial class SummedOverHodographScanCtrl : UserControl
    {

        private SummedOverHodographScanViewModel _viewModel;
        public SummedOverHodographScanViewModel ViewModel => _viewModel;

        public SummedOverHodographScanCtrl()
        {
            InitializeComponent();

            _viewModel = new SummedOverHodographScanViewModel();
            DataContext = _viewModel;
        }
    }
}
