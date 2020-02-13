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
using SummedScanModule.ViewModels;

namespace IntegratedScanModule.Views
{
    /// <summary>
    /// Interaction logic for SummedScanOptionsCtrl.xaml
    /// </summary>
    public partial class SummedScanOptionsCtrl : UserControl
    {
        private SummedScanOptionsViewModel _viewModel;
        public SummedScanOptionsViewModel ViewModel => _viewModel;


        public SummedScanOptionsCtrl()
        {
            InitializeComponent();

            _viewModel = new SummedScanOptionsViewModel();
            DataContext = _viewModel;
        }
    }
}
