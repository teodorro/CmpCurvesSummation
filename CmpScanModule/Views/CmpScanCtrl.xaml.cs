using System.Windows.Controls;
using CmpCurvesSummation.Core;

namespace CmpScanModule.Views
{
    /// <summary>
    /// Interaction logic for CmpScanCtrl.xaml
    /// </summary>
    public partial class CmpScanCtrl : UserControl
    {
        public CmpScanCtrl()
        {
            InitializeComponent();
            EventAggregator.Instance.FileLoaded += (o, args) => { this.IsEnabled = true; };
        }
    }
}
