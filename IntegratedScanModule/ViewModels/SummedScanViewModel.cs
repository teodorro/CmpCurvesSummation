using System;
using System.Linq;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public class SummedScanViewModel
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScan _summedScan;

        public PlotModel Plot { get; private set; }


        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "После суммирования" };
            TestScan();
            SetAxes();
        }


        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScan = args.CmpScan;

            _summedScan = new SummedScan(_cmpScan);
            LoadSummedScan();
        }

        private void SumScan(ICmpScan cmpScan)
        {
            throw new NotImplementedException();
        }

//        public void DataLoaded(object obj, SummedOverHodographEventArgs args)
//        {
//            _summedOverHodographScan = args.SummedScan;
//
//            LoadCmpScan();
//        }

        private void LoadSummedScan()
        {
            LoadSeries();
            UpdateAxes();
            Plot.InvalidatePlot(true);
        }

        // TODO: not clear wtf. If no cmpScan in the beginning - no axes. If no axes - update doesn't work - bad plot
        private void UpdateAxes()
        {
            if (Plot.Axes.Count == 0)
            {
                SetAxes();
            }

            if (Plot.Axes.Count == 1)
                return;

            if (Plot.Axes.All(x => x.Position != AxisPosition.Top))
                Plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = Plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = 0;
            top.AbsoluteMaximum = _summedScan.VelocityLength;

            var left = Plot.Axes.First(x => x.Position == AxisPosition.Left);
            left.AbsoluteMinimum = 0;
            left.AbsoluteMaximum = _summedScan.AscanLength;
            left.StartPosition = 1;
            left.EndPosition = 0;
        }

        private void LoadSeries()
        {
            Plot.Series.Clear();

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = _summedScan.VelocityLength,
                Y0 = 0,
                Y1 = _summedScan.AscanLength,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = GetDataArray()
            };

            Plot.Series.Add(heatMapSeries);
        }

        // TODO: different palettes and other
        public void SetAxes()
        {
            //Plot.Axes.Clear();
            Plot.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Rainbow(colorsCount)
            });
        }

        private double[,] GetDataArray()
        {
            var res = new double[_summedScan.VelocityLengthDimensionless, _summedScan.AscanLengthDimensionless];

            for (int i = 0; i < _summedScan.VelocityLengthDimensionless; i++)
                for (int j = 0; j < _summedScan.AscanLengthDimensionless; j++)
                    res[i, j] = _summedScan.Data[i][j];

            return res;
        }


        private void TestScan()
        {
            Plot.Series.Clear();

            // generate 1d normal distribution
            var singleData = new double[100];
            for (int x = 0; x < 100; ++x)
            {
                singleData[x] = Math.Exp((-1.0 / 2.0) * Math.Pow(((double)x - 50.0) / 20.0, 2.0));
            }

            // generate 2d normal distribution
            var data = new double[100, 100];
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 0; y < 100; ++y)
                {
                    data[y, x] = singleData[x] * singleData[(y + 30) % 100] * 100;
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = 99,
                Y0 = 0,
                Y1 = 99,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data
            };


            Plot.Series.Add(heatMapSeries);
        }
    }
}