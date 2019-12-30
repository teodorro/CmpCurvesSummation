using CmpCurvesSummation.Core;
using OxyPlot;

namespace IntegratedScanModule.ViewModels
{
    public class IntegratedScanViewModel
    {
        private const int colorsCount = 1024;

        private IIntegratedCmpScan _integratedScan;
        public PlotModel Plot { get; private set; }


        public IntegratedScanViewModel()
        {
            Plot = new PlotModel { Title = "Годограф" };
            TestScan();
            SetAxes();
        }
    }
}