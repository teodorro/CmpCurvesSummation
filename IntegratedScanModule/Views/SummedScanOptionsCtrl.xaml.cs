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
using CmpCurvesSummation.Core;

namespace IntegratedScanModule.Views
{
    /// <summary>
    /// Interaction logic for SummedScanOptionsCtrl.xaml
    /// </summary>
    public partial class SummedScanOptionsCtrl : UserControl
    {
        public SummedScanOptionsCtrl()
        {
            InitializeComponent();
            EventAggregator.Instance.FileLoaded += (o, args) => { IsEnabled = false; };
            EventAggregator.Instance.SummationFinished += (o, args) => { IsEnabled = true; };
        }
    }
}
