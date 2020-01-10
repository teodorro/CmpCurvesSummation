using System;
using System.Linq;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public delegate void HodographDrawClickHander(object obj, HodographDrawVTClickEventArgs e);

    public class SummedScanViewModel
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        public PlotModel Plot { get; }

        public event HodographDrawClickHander HodographDrawClick;



        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "После суммирования" };

            TestScan();
            TestAnnotations();

            SetAxes();
            Plot.MouseDown += PlotOnMouseDown;
        }


        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScan = args.CmpScan;

            _summedScan = new SummedScanVT(_cmpScan);
            LoadSummedScan();
        }

        
        private void LoadSummedScan()
        {
            LoadSeries();
            UpdateAxes();

            TestAnnotations();
            
            Plot.InvalidatePlot(true); // refresh plot?
        }

        private void PlotOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == OxyMouseButton.Left)
            {
                Axis X_Axis = null;
                Axis Y_Axis = null;
                
                var axisList = Plot.Axes;

                foreach (var ax in axisList)
                {
                    if (ax.Position == AxisPosition.Top)
                        X_Axis = ax;
                    else if (ax.Position == AxisPosition.Left)
                        Y_Axis = ax;
                }

                var p = Axis.InverseTransform(e.Position, X_Axis, Y_Axis);
                var v = Math.Round(p.X, 4);
                var t = Math.Round(p.Y, 2);
                var point = new PointAnnotation()
                {
                    Fill = OxyColor.FromRgb(0, 0, 0),
                    X = v,
                    Y = t
                };
                point.Size = 2;

                Plot.Annotations.Add(point);
                Plot.InvalidatePlot(true);

                HodographDrawClick?.Invoke(this, new HodographDrawVTClickEventArgs(v, t));
            }
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
            top.AbsoluteMinimum = _summedScan.MinVelocity;
            top.AbsoluteMaximum = _summedScan.MaxVelocity;

            var left = Plot.Axes.First(x => x.Position == AxisPosition.Left);
            //            left.AbsoluteMinimum = _summedScan.MinHeight;
            //            left.AbsoluteMaximum = _summedScan.MaxHeight;
                        left.AbsoluteMinimum = _summedScan.MinTime;
                        left.AbsoluteMaximum = _summedScan.MaxTime;
            left.StartPosition = 1;
            left.EndPosition = 0;
        }

        private void LoadSeries()
        {
            Plot.Series.Clear();

            var heatMapSeries = new HeatMapSeries
            {
                X0 = _summedScan.MinVelocity,
                X1 = _summedScan.MaxVelocity,
                //                Y0 = _summedScan.MinHeight,
                //                Y1 = _summedScan.MaxHeight,
                                Y0 = _summedScan.MinTime,
                                Y1 = _summedScan.MaxTime,
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = _summedScan.GetDataArray()
            };

            Plot.Series.Add(heatMapSeries);
        }

        // TODO: different palettes and other
        public void SetAxes()
        {
            //Plot.Axes.Clear();
            Plot.Axes.Add(new LinearColorAxis
            {
                                Palette = OxyPalettes.Jet(colorsCount)
//                Palette = OxyPalettes.Gray(colorsCount)
            });
        }

        public void OnDeleteClick(object obj, DeleteLayerEventsArgs e)
        {
            var annotation = Plot.Annotations.FirstOrDefault(
                x => (x as PointAnnotation)?.Y == e.Time && (x as PointAnnotation)?.X == e.Velocity);
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

        private void TestAnnotations()
        {
            var annotation = new TextAnnotation
            {
                Text = "hodograph",
                TextPosition = new DataPoint(0, 22)
            };
            Plot.Annotations.Add(annotation);

            var a2 = new EllipseAnnotation()
            {
                Fill = OxyColor.FromArgb(100, 255, 255, 255),

                X = 43,
                Y = 55,
                Height = 10,
                Width = 10
            };

            Plot.Annotations.Add(a2);

            var a4 = new PolylineAnnotation();
            a4.Points.Add(new DataPoint(10, 22));
            a4.Points.Add(new DataPoint(13, 27));
            a4.Points.Add(new DataPoint(30, 42));
            a4.Points.Add(new DataPoint(22, 68));
            a4.Points.Add(new DataPoint(20, 72));
            a4.Color = OxyColor.FromRgb(255, 255, 255);
            a4.InterpolationAlgorithm = new CanonicalSpline(0.5);
            Plot.Annotations.Add(a4);
        }
    }
}