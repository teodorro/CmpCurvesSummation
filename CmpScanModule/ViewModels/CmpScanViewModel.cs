using System;
using System.Linq;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;
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
            
            TestScan();
            SetAxes();
        }


        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScan = args.CmpScan;
            LoadCmpScan();
        }

        private void LoadCmpScan()
        {
            LoadSeries();
            UpdateAxes();
            Plot.InvalidatePlot(true);
        }

        // TODO: not clear wtf. If no cmpScan in the beginning - no axes. If no axes - update doesn't work - bad plot
        private void UpdateAxes()
        {
            if (Plot.Axes.Count == 0)
                SetAxes();

            if (Plot.Axes.Count == 1)
                return;
            
            if (Plot.Axes.All(x => x.Position != AxisPosition.Top))
                Plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = Plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = 0;
            top.AbsoluteMaximum = _cmpScan.Length;

            var left = Plot.Axes.First(x => x.Position == AxisPosition.Left);
            left.AbsoluteMinimum = 0;
            left.AbsoluteMaximum = _cmpScan.AscanLength;
            left.StartPosition = 1;
            left.EndPosition = 0;
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
            heatMapSeries.Interpolate = false;
            Plot.Series.Add(heatMapSeries);
        }

        // TODO: different palettes and other
        public void SetAxes()
        {
            Plot.Axes.Add(new LinearColorAxis
            {
                                Palette = OxyPalettes.Jet(colorsCount)
//                Palette = OxyPalettes.Gray(colorsCount)
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

        public void OnHodographDrawClick(object obj, HodographDrawVTClickEventArgs e)
        {
            var h = e.Velocity * e.Time;
            var v = e.Velocity;
            var hodograph = new double[_cmpScan.LengthDimensionless];
            var hodographCurve = new PolylineAnnotation();
            for (int i = 0; i < _cmpScan.LengthDimensionless; i++)
            {
                var d = i * _cmpScan.StepTime;
                hodograph[i] = Math.Round(CmpMath.Instance.HodographLineLoza(d, h, v), 4);
                hodographCurve.Points.Add(new DataPoint(d, hodograph[i]));
            }

            hodographCurve.Color = OxyColor.FromRgb(0, 0, 0);
            hodographCurve.InterpolationAlgorithm = new CanonicalSpline(0.5);
            hodographCurve.LineStyle = LineStyle.Solid;
            Plot.Annotations.Add(hodographCurve);

            Plot.InvalidatePlot(true);
        }

        public void OnDeleteClick(object obj, DeleteLayerEventsArgs e)
        {
            var h = Math.Round(e.Velocity * e.Time, 4);
            var t = Math.Round(CmpMath.Instance.HodographLineLoza(0, h, e.Velocity), 2);
            var annotation = Plot.Annotations.FirstOrDefault(x => (x as PolylineAnnotation)?.Points[0].Y == t);
            Plot.Annotations.Remove(annotation);
            Plot.InvalidatePlot(true);
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

//        public void OnHodographDrawClick(object obj, HodographDrawVHClickEventArgs e)
//        {
//            var hodograph = new PolylineAnnotation();
//            hodograph.Points.Add(new DataPoint(e.Velocity, e.Height));
//            hodograph.Points.Add(new DataPoint(e.Velocity + 2, e.Height + 5));
//            hodograph.Points.Add(new DataPoint(e.Velocity + 4, e.Height + 15));
//            hodograph.Color = OxyColor.FromRgb(255, 255, 255);
//            hodograph.InterpolationAlgorithm = new CanonicalSpline(0.5);
//            hodograph.LineStyle = LineStyle.Solid;
//            Plot.Annotations.Add(hodograph);
//
//            Plot.InvalidatePlot(true);
//        }

        
    }
}