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
using ProcessingModule.Processing.SummedScan;
using ProcessingModule.ViewModels;
using ProcessingModule.ViewModels.SummedScan;

namespace ProcessingModule.Views.SummedScan
{
    /// <summary>
    /// Interaction logic for RaiseToPowerCtrl.xaml
    /// </summary>
    public partial class RaiseToPowerCtrl : UserControl
    {
        private RaiseToPowerViewModel _viewModel;

        public RaiseToPowerCtrl(SumProcessingListChangedHandler onSumProcessingListChanged, RaiseToPower processing)
        {
            InitializeComponent();

            _viewModel = new RaiseToPowerViewModel(processing);
            _viewModel.ProcessingListChanged += onSumProcessingListChanged;
            DataContext = _viewModel;
        }
    }
}
