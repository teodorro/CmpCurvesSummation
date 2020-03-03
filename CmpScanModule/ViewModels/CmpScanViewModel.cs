using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CmpScanModule.ViewModels
{

    public class CmpScanViewModel
    {
        private const double _hodographCurveStrokeThickness = 1;

        private ICmpScan _cmpScan;
        private PaletteType _palette = PaletteType.Jet;
        private bool _interpolate;

        public PlotModel Plot { get; private set; }

        private OxyColor _hodographColor = OxyColor.FromRgb(0, 0, 0);

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
            Plot = new PlotModel {Title = "Годограф"};
            Plot.MouseDown += PlotOnMouseDown;
            EventAggregator.Instance.PlotVisualOptionsChanged += OnPlotVisualOptionsChanged;
            EventAggregator.Instance.CmpScanParametersChanged += OnCmpScanParametersChanged;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
            EventAggregator.Instance.SumDataProcessed += OnSumProcessed;
            EventAggregator.Instance.SummationFinished += OnSummationFinished;
        }


        private Axis TimeAxis => Plot.Axes.FirstOrDefault(x => x.Position == AxisPosition.Left);
        private Axis DistanceAxis => Plot.Axes.FirstOrDefault(x => x.Position == AxisPosition.Top);
        private HeatMapSeries HeatMap => Plot.Series.FirstOrDefault() as HeatMapSeries;

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Plot.Annotations.Clear();
        }

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
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

        private void AddPalette(PaletteType palette)
        {
            var converter = new PaletteToOxyConverter();
            Plot.Axes.Add(new LinearColorAxis {Palette = (OxyPalette)converter.Convert(palette, null, null, null)});
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
                    hodograph[i] = CmpMath.Instance.HodographLineLoza(distance, h, velocity);
                    hodographCurve.Points.Add(new DataPoint(distance, hodograph[i]));
                }

                hodographCurve.Color = HodographColor;
                hodographCurve.InterpolationAlgorithm = new CanonicalSpline(0.5);
                hodographCurve.LineStyle = LineStyle.Solid;
                hodographCurve.StrokeThickness = _hodographCurveStrokeThickness;
                Plot.Annotations.Add(hodographCurve);
            }
        }

        private void OnPlotVisualOptionsChanged(object obj, PlotVisualOptionsChangedEventArgs e)
        {
            _palette = e.Palette;
            _interpolate = e.Interpolation;

            HodographColor = e.ShowHodographs 
                ? OxyColor.FromArgb(e.ColorHodograph.A, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B)
                : OxyColor.FromArgb(0, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B);
            
            if (Plot == null)
                return;
            if (Plot.Axes.Any(x => x is LinearColorAxis))
                Plot.Axes.Remove(Plot.Axes.First(x => x is LinearColorAxis));
            AddPalette(_palette);

            if (HeatMap != null)
                HeatMap.Interpolate = e.Interpolation;

            Plot.InvalidatePlot(true);
        }

        private void PlotOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == OxyMouseButton.Left && TimeAxis != null)
            {
                var point = GetPointFromOxyPosition(e);
                if (point.X < 0)
                    ChangeTimeOffset(point);
            }
        }

        private void ChangeTimeOffset(DataPoint point)
        {
            var oldMinTime = _cmpScan.MinTime;
            var offset = point.Y;
            ModifyAxesTimeOffset(offset);
            _cmpScan.MinTime -= offset;

            Plot.Annotations.Clear();
            Plot.InvalidatePlot(true);

            EventAggregator.Instance.Invoke(this,
                new CmpScanParametersChangedEventArgs(_cmpScan.StepDistance, _cmpScan.StepTime, oldMinTime,
                    _cmpScan.StepDistance, _cmpScan.StepTime, _cmpScan.MinTime));
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

        private void OnCmpScanParametersChanged(object sender, CmpScanParametersChangedEventArgs e)
        {
            Plot.Annotations.Clear();
            UpdateDistanceDirection(e.OldStepDistance, e.StepDistance);
            UpdateTimeDirection(e.OldStepTime, e.StepTime);
            Plot.InvalidatePlot(true);
        }

        private void UpdateDistanceDirection(double oldStepDistance, double stepDistance)
        {
            var factor = stepDistance / oldStepDistance;
            DistanceAxis.AbsoluteMinimum *= factor;
            DistanceAxis.AbsoluteMaximum *= factor;
            HeatMap.X0 *= factor;
            HeatMap.X1 *= factor;
        }

        private void UpdateTimeDirection(double oldStepTime, double stepTime)
        {
            var factor = stepTime / oldStepTime;
            TimeAxis.AbsoluteMinimum *= factor;
            TimeAxis.AbsoluteMaximum *= factor;
            HeatMap.Y0 *= factor;
            HeatMap.Y1 *= factor;
        }

        private void OnSumProcessed(object obj, SumDataProcessedEventArgs e)
        {
            OnRefreshLayers(obj, new RefreshLayersEventArgs(e.SumScan.Layers));
        }

        private void RepaintHodographs()
        {
            if (!Plot.Annotations.Any(x => x is PolylineAnnotation))
                return;
            foreach (var hodograph in Plot.Annotations.Where(x => x is PolylineAnnotation))
                (hodograph as PolylineAnnotation).Color = HodographColor;

            Plot.InvalidatePlot(true);
        }

        private void OnSummationFinished(object obj, SummationFinishedEventArgs e)
        {
            e.SummedScan.RefreshLayers += OnRefreshLayers;
        }
    }


    public class PaletteToOxyConverter : IValueConverter
    {
        private const int _colorsCount = 512;


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var palette = (PaletteType) value;
            OxyPalette paletteOxy;
            switch (palette)
            {
                case PaletteType.Gray:
                    paletteOxy = OxyPalettes.Gray(_colorsCount);
                    break;
                case PaletteType.Rainbow:
                    paletteOxy = OxyPalettes.Rainbow(_colorsCount);
                    break;
                case PaletteType.Hot:
                    paletteOxy = OxyPalettes.Hot(_colorsCount);
                    break;
                case PaletteType.Cool:
                    paletteOxy = OxyPalettes.Cool(_colorsCount);
                    break;
                case PaletteType.HueDistinct:
                    paletteOxy = OxyPalettes.HueDistinct(_colorsCount);
                    break;
                case PaletteType.Hue:
                    paletteOxy = OxyPalettes.Hue(_colorsCount);
                    break;
                case PaletteType.BlackWhiteRed:
                    paletteOxy = OxyPalettes.BlackWhiteRed(_colorsCount);
                    break;
                case PaletteType.BlueWhiteRed:
                    paletteOxy = OxyPalettes.BlueWhiteRed(_colorsCount);
                    break;
                default:
                    paletteOxy = OxyPalettes.Jet(_colorsCount);
                    break;
            }

            return paletteOxy;

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}