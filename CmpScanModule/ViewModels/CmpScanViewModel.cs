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
        private PaletteType _palette = PaletteType.Jet;
        public PlotModel Plot { get; private set; }

        
        public CmpScanViewModel()
        {
            Plot = new PlotModel { Title = "Годограф" };
        }


        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScan = args.CmpScan;
            LoadSeries();
            UpdateAxes();
        }
        
        private void UpdateAxes()
        {
            Plot.InvalidatePlot(true); // to make axes be created

            if (!Plot.Axes.Any(x => x is LinearColorAxis))
                AddPalette(_palette);

            if (Plot.Axes.Count == 1)
                return;

            TuneHorizontalAxis();
            TuneVerticalAxis();

            Plot.InvalidatePlot(true); // to update axes in UI
        }

        private void TuneVerticalAxis()
        {
            var left = Plot.Axes.First(x => x.Position == AxisPosition.Left);
            left.StartPosition = 1;
            left.EndPosition = 0;
            left.AbsoluteMinimum = 0;
            left.AbsoluteMaximum = _cmpScan.AscanLength;
        }

        private void TuneHorizontalAxis()
        {
            if (Plot.Axes.All(x => x.Position != AxisPosition.Top))
                Plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = Plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = 0;
            top.AbsoluteMaximum = _cmpScan.Length;
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

        public void AddPalette(PaletteType palette)
        {
            var oxyPalette = OxyPalettes.Jet(colorsCount);
            switch (palette)
            {
                case PaletteType.Gray:
                    oxyPalette = OxyPalettes.Gray(colorsCount);
                    break;
                case PaletteType.BW:
                    oxyPalette = OxyPalettes.Gray(2);
                    break;
            }
            Plot.Axes.Add(new LinearColorAxis{ Palette = oxyPalette });
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

        public void OnDeleteClick(object obj, DeleteLayerEventArgs e)
        {
            var h = Math.Round(e.Velocity * e.Time, 4);
            var t = Math.Round(CmpMath.Instance.HodographLineLoza(0, h, e.Velocity), 2);
            var annotation = Plot.Annotations.FirstOrDefault(x => (x as PolylineAnnotation)?.Points[0].Y == t);
            Plot.Annotations.Remove(annotation);
            Plot.InvalidatePlot(true);
        }

        public void OnPaletteChanged(object obj, PaletteChangedEventArgs e)
        {
            _palette = e.Palette;
            if (Plot == null)
                return;
            if (Plot.Axes.Any(x => x is LinearColorAxis))
                Plot.Axes.Remove(Plot.Axes.First(x => x is LinearColorAxis));
            if (!Plot.Axes.Any(x => x is LinearColorAxis))
                AddPalette(_palette);
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