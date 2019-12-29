using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CmpScanModule.ViewModels
{
    public class CmpScanViewModel
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        public PlotModel Plot { get; private set; }


        public CmpScanViewModel()
        {
            Plot = new PlotModel { Title = "Годограф" };
            
        }


        public void DataLoaded(object obj, FileLoadedEventArgs args)
        {
            _cmpScan = args.CmpScan;
            LoadCmpScan();
        }

        public void LoadCmpScan()
        {
            LoadSeries();
            SetAxes();
        }

        private void LoadSeries()
        {
            Plot.Series.Clear();

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = _cmpScan.Length,
                Y0 = 0,
                Y1 = _cmpScan.AscanLength,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = GetDataArray()
            };

            Plot.Series.Add(heatMapSeries);
        }

        // TODO: different palettes and other
        public void SetAxes()
        {
            Plot.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(colorsCount)
            });
        }

        private double[,] GetDataArray()
        {
            var res = new double[_cmpScan.LengthDimensionless, _cmpScan.AscanLengthDimensionless];

            for (int i = 0; i < _cmpScan.LengthDimensionless; i++)
            for (int j = 0; j < _cmpScan.AscanLengthDimensionless; j++)
                res[i, j] = _cmpScan.Data[i][j];

            return res;
        }
    }
}