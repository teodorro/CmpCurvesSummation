using System;
using System.Linq;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public delegate void HodographDrawClickHander(object obj, HodographDrawVTClickEventArgs e);
    public delegate void EndSummationHandler(object obj, SummationFinishedEventArgs e);


    public class SummedScanViewModel
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        private PaletteType _palette = PaletteType.Jet;

        public PlotModel Plot { get; }
        internal bool AutoSummation { get; set; }

        public event HodographDrawClickHander HodographDrawClick;
        public event EndSummationHandler SummationFinished;


        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "После суммирования" };

            Plot.MouseDown += PlotOnMouseDown;
        }


        private Axis TimeAxis => Plot.Axes.First(x => x.Position == AxisPosition.Left);

        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            _cmpScan = args.CmpScan;

            if (AutoSummation)
                Sum();
        }

        private async void Sum()
        {
            _summedScan = new SummedScanVT(_cmpScan);
            await Task.Run(() =>
            {
                _summedScan.Sum(_cmpScan);
            }).ContinueWith(task =>
            {
                LoadSummedScan();
            });
            SummationFinished?.Invoke(this, new SummationFinishedEventArgs());
        }


        private void LoadSummedScan()
        {
            LoadSeries();
            UpdateAxes();
            
            Plot.InvalidatePlot(true); // refresh plot?
        }

        private void PlotOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == OxyMouseButton.Left 
                                  && TimeAxis != null)
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

                var point = Axis.InverseTransform(e.Position, X_Axis, Y_Axis);

                if (IsPointOnPlot(point) && NoPointWithSameTime(point))
                {
                    var velocity = Math.Round(point.X, 3);
                    var time = Math.Round(point.Y, 2);
                    AddHodographToPlot(velocity, time);
                }
            }
        }

        private bool NoPointWithSameTime(DataPoint point)
        {
            var points = Plot.Annotations.OfType<PointAnnotation>();
            return (points != null || points.Any(x => x.Y == point.Y));
        }

        private void ChangeTimeOffset(double time)
        {
            TimeAxis.AbsoluteMinimum += time - TimeAxis.AbsoluteMinimum;
            TimeAxis.AbsoluteMaximum += time - TimeAxis.AbsoluteMinimum;

            Plot.InvalidatePlot(true);
        }

        private bool IsTimeOffsetChangeArea(DataPoint point)
        {
            var v = Math.Round(point.X, 3);
            if (v < CmpMath.Instance.WaterVelocity || v >= CmpMath.SpeedOfLight / 2)
                return true;
            return false;
        }

        private void AddHodographToPlot(double velocity, double time)
        {
            var point = new PointAnnotation()
            {
                Fill = OxyColor.FromRgb(0, 0, 0),
                X = velocity,
                Y = time
            };
            point.Size = 2;

            Plot.Annotations.Add(point);
            Plot.InvalidatePlot(true);

            HodographDrawClick?.Invoke(this, new HodographDrawVTClickEventArgs(velocity, time));
        }

        private bool IsPointOnPlot(DataPoint point)
        {
            var v = Math.Round(point.X, 3);
            var t = Math.Round(point.Y, 2);
            if (v < CmpMath.Instance.WaterVelocity || v >= CmpMath.SpeedOfLight)
                return false;
            if (t < TimeAxis.AbsoluteMinimum || t >= TimeAxis.AbsoluteMaximum)
                return false;
            return true;
        }

        private void UpdateAxes()
        {
            Plot.InvalidatePlot(true); // to make axes be created

            if (!Plot.Axes.Any(x => x is LinearColorAxis))
                AddPalette(_palette);

            if (Plot.Axes.Count == 1)
                return;

            TuneHorizontalAxis();
            TuneVerticalAxis(_cmpScan.MinTime, _cmpScan.MaxTime);

            Plot.InvalidatePlot(true); // to update axes in UI
        }

        private void TuneVerticalAxis(double min, double max)
        {
            var left = Plot.Axes.First(x => x.Position == AxisPosition.Left);
            left.StartPosition = 1;
            left.EndPosition = 0;
            left.AbsoluteMinimum = min;
            left.AbsoluteMaximum = max;
        }

        private void TuneHorizontalAxis()
        {
            if (Plot.Axes.All(x => x.Position != AxisPosition.Top))
                Plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = Plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = _summedScan.MinVelocity;
            top.AbsoluteMaximum = _summedScan.MaxVelocity;
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
            Plot.Axes.Add(new LinearColorAxis { Palette = oxyPalette });
        }

        private void LoadSeries()
        {
            Plot.Series.Clear();

            if (_summedScan != null)
            {
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
            else
            {
                Plot.Series.Add(new HeatMapSeries());
            }
        }

        public void OnDeleteClick(object obj, DeleteLayerEventArgs e)
        {
            var annotation = Plot.Annotations.FirstOrDefault(
                x => (x as PointAnnotation)?.Y == e.Time && (x as PointAnnotation)?.X == e.Velocity);
            Plot.Annotations.Remove(annotation);
            Plot.InvalidatePlot(true);
        }

        public void OnAutoSummationChange(object sender, AutoSummationCheckEventArgs e)
        {
            AutoSummation = e.Auto;
        }

        public void OnSummationStarted(object obj, EventArgs e)
        {
            Sum();
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

        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Plot.Series.Clear();
            Plot.Axes.Clear();
            Plot.Annotations.Clear();
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