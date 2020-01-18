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
            
            Plot.MouseDown += PlotOnMouseDown;
        }


        private Axis TimeAxis => Plot.Axes.FirstOrDefault(x => x.Position == AxisPosition.Left);
        private Axis DistanceAxis => Plot.Axes.FirstOrDefault(x => x.Position == AxisPosition.Top);
        private HeatMapSeries HeatMap => Plot.Series.First() as HeatMapSeries;


        public void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Plot.Annotations.Clear();
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
            TuneVerticalAxis(0, _cmpScan.AscanLength);

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
            var h = CmpMath.Instance.Depth(e.Velocity, e.Time);
            var v = e.Velocity;
            var hodograph = new double[_cmpScan.LengthDimensionless];
            var hodographCurve = new PolylineAnnotation();
            for (int i = 0; i < _cmpScan.LengthDimensionless; i++)
            {
                var d = i * _cmpScan.StepTime;
                hodograph[i] = Math.Round(CmpMath.Instance.HodographLineLoza(d, h, v), 2);
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
            var h = CmpMath.Instance.Depth(e.Velocity, e.Time);
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

        private void PlotOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == OxyMouseButton.Left
                                  && TimeAxis != null)
            {
                var point = GetPointFromOxyPosition(e);

                if (IsTimeOffsetChangeArea(point))
                {
                    var offset = Math.Round(point.Y, 2);
                    ModifyAxesTimeOffset(offset);
                    _cmpScan.MinTime -= offset;
                    Plot.InvalidatePlot(true);
                }
            }
        }

        private void ModifyAxesTimeOffset(double offset)
        {
            HeatMap.Y0 -= offset;
            HeatMap.Y1 -= offset;
            TimeAxis.AbsoluteMinimum -= offset;
            TimeAxis.AbsoluteMaximum -= offset;
        }

        private DataPoint GetPointFromOxyPosition(OxyMouseDownEventArgs e)
            => Axis.InverseTransform(e.Position, DistanceAxis, TimeAxis);

        private bool IsTimeOffsetChangeArea(DataPoint point)
        {
            var v = Math.Round(point.X, 3);
            if (v < CmpMath.Instance.WaterVelocity || v >= CmpMath.SpeedOfLight / 2)
                return true;
            return false;
        }
        
        public void OnStepDistanceChanged(object sender, StepDistanceEventArgs e)
        {
            var factor = e.NewStepDistance / e.OldStepDistance;
            DistanceAxis.AbsoluteMinimum *= factor;
            DistanceAxis.AbsoluteMaximum *= factor;
            HeatMap.X0 *= factor;
            HeatMap.X1 *= factor;
            Plot.InvalidatePlot(true);
        }

        public void OnStepTimeChanged(object sender, StepTimeEventArgs e)
        {
            var factor = e.NewStepTime / e.OldStepTime;
            TimeAxis.AbsoluteMinimum *= factor;
            TimeAxis.AbsoluteMaximum *= factor;
            HeatMap.Y0 *= factor;
            HeatMap.Y1 *= factor;
            Plot.InvalidatePlot(true);
        }

    }
}