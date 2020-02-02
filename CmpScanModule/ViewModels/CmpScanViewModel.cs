using System;
using System.Collections.Generic;
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
        private const double _hodographCurveStrokeThickness = 0.5;

        private ICmpScan _cmpScan;
        private PaletteType _palette = PaletteType.Jet;
        private bool _interpolate;

        public PlotModel Plot { get; private set; }

        public event TimeOffsetChangedHandler TimeOffsetChanged;

        private OxyColor _hodographColor = OxyColor.FromRgb(255, 255, 255);
        public OxyColor HodographColor
        {
            get => _hodographColor;
            set
            {
                _hodographColor = value;
                RepaintHodographs();
            }
        }


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
            Plot.Annotations.Clear();
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
            left.Title = "T";
            left.TitleFontSize = 1;
        }

        private void TuneHorizontalAxis()
        {
            if (Plot.Axes.All(x => x.Position != AxisPosition.Top))
                Plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = Plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = 0;
            top.AbsoluteMaximum = _cmpScan.Length;
            top.Title = "D";
            top.TitleFontSize = 1;
        }

        private void LoadSeries()
        {
            Plot.Series.Clear();
            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = _cmpScan.Length,
                Y0 = _cmpScan.MinTime,
                Y1 = _cmpScan.MaxTime,
                Interpolate = _interpolate,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = GetDataArray()
            };
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
                case PaletteType.Rainbow:
                    oxyPalette = OxyPalettes.Rainbow(colorsCount);
                    break;
                case PaletteType.Hot:
                    oxyPalette = OxyPalettes.Hot(colorsCount);
                    break;
                case PaletteType.HueDistinct:
                    oxyPalette = OxyPalettes.HueDistinct(colorsCount);
                    break;
                case PaletteType.Hue:
                    oxyPalette = OxyPalettes.Hue(colorsCount);
                    break;
                case PaletteType.BlackWhiteRed:
                    oxyPalette = OxyPalettes.BlackWhiteRed(colorsCount);
                    break;
                case PaletteType.BlueWhiteRed:
                    oxyPalette = OxyPalettes.BlueWhiteRed(colorsCount);
                    break;
                case PaletteType.Cool:
                    oxyPalette = OxyPalettes.Cool(colorsCount);
                    break;
                case PaletteType.Jet:
                    oxyPalette = OxyPalettes.Jet(colorsCount);
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

        private void OnRefreshLayers(object o, RefreshLayersEventArgs e)
        {
            Plot.Annotations.Clear();
            RefreshHodographCurves(e.Layers);
            Plot.InvalidatePlot(true);
        }

        private void RefreshHodographCurves(IEnumerable<Tuple<double, double>> layers)
        {
            foreach (var layer in layers)
            {
                var velocity = layer.Item1;
                var time = layer.Item2;
                var h = CmpMath.Instance.Depth(velocity, time);
                var hodograph = new double[_cmpScan.LengthDimensionless];
                var hodographCurve = new PolylineAnnotation();
                for (int i = 0; i < _cmpScan.LengthDimensionless; i++)
                {
                    var distance = i * _cmpScan.StepTime;
                    hodograph[i] = Math.Round(CmpMath.Instance.HodographLineLoza(distance, h, velocity), 2);
                    hodographCurve.Points.Add(new DataPoint(distance, hodograph[i]));
                }

                hodographCurve.Color = HodographColor;
                hodographCurve.InterpolationAlgorithm = new CanonicalSpline(0.5);
                hodographCurve.LineStyle = LineStyle.Solid;
                hodographCurve.StrokeThickness = _hodographCurveStrokeThickness;
                Plot.Annotations.Add(hodographCurve);
            }
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

                if (point.X < 0)
                    ChangeTimeOffset(point);
            }
        }

        private void ChangeTimeOffset(DataPoint point)
        {
            var offset = Math.Round(point.Y, 2);
            ModifyAxesTimeOffset(offset);
            _cmpScan.MinTime -= offset;

            Plot.Annotations.Clear();
            Plot.InvalidatePlot(true);

            TimeOffsetChanged?.Invoke(this, new TimeOffsetChangedEventArgs(offset, _cmpScan));
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
        
        public void OnStepDistanceChanged(object sender, StepDistanceEventArgs e)
        {
            Plot.Annotations.Clear();
            var factor = e.NewStepDistance / e.OldStepDistance;
            DistanceAxis.AbsoluteMinimum *= factor;
            DistanceAxis.AbsoluteMaximum *= factor;
            HeatMap.X0 *= factor;
            HeatMap.X1 *= factor;
            Plot.InvalidatePlot(true);
        }

        public void OnStepTimeChanged(object sender, StepTimeEventArgs e)
        {
            Plot.Annotations.Clear();
            var factor = e.NewStepTime / e.OldStepTime;
            TimeAxis.AbsoluteMinimum *= factor;
            TimeAxis.AbsoluteMaximum *= factor;
            HeatMap.Y0 *= factor;
            HeatMap.Y1 *= factor;
            Plot.InvalidatePlot(true);
        }

        public void OnSummationFinished(object obj, SummationFinishedEventArgs e)
        {
            Plot.Annotations.Clear();
            e.SummedScan.RefreshLayers += OnRefreshLayers;
            Plot.InvalidatePlot(true);
        }

        private void RepaintHodographs()
        {
            if (!Plot.Annotations.Any(x => x is PolylineAnnotation))
                return;
            foreach (var hodograph in Plot.Annotations.Where(x => x is PolylineAnnotation))
            {
                (hodograph as PolylineAnnotation).Color = HodographColor;
            }

            Plot.InvalidatePlot(true);
        }

        public void OnHodographColorChanged(object obj, HodographColorChangedEventArgs e)
        {
            HodographColor = OxyColor.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
        }

        public void OnInterpolationChanged(object obj, InterpolationChangedEventArgs e)
        {
            if (Plot == null)
                return;
            _interpolate = e.Interpolation;
            HeatMap.Interpolate = e.Interpolation;
            Plot.InvalidatePlot(true);
        }
    }
}