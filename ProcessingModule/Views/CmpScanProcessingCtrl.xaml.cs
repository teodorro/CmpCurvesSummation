using System.Windows.Controls;
using CmpCurvesSummation.Core;

namespace ProcessingModule.Views
{
    /// <summary>
    /// Interaction logic for CmpScanProcessingCtrl.xaml
    /// </summary>
    public partial class CmpScanProcessingCtrl : UserControl
    {

        public CmpScanProcessingCtrl()
        {
            InitializeComponent();

            EventAggregator.Instance.FileLoaded += (o, args) => { this.IsEnabled = true; };
        }


    }
}
