using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using LayersInfoModule.Annotations;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public interface ISummedScanViewModel
    {
        PlotModel Plot { get; }
        OxyColor AvgLinesColor { get; set; }
        void AddPalette(PaletteType palette);
    }



    public class SummedScanViewModel : ISummedScanViewModel, INotifyPropertyChanged
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        private PaletteType _palette = PaletteType.Jet;
        private IPlotLoader _plotLoader;
        private ILayersLoader _layersLoader;
        private int _halfWaveSize = 5;
        private bool _autoCorrection;

        public PlotModel Plot { get; }
        public double MaxVelocity { get; private set; } = CmpMath.PlotMaxVelocity;
        
        private OxyColor _avgLinesColor = OxyColor.FromRgb(0, 0, 0);
        public OxyColor AvgLinesColor
        {
            get => _avgLinesColor;
            set
            {
                _avgLinesColor = value;
                RepaintAvgPoints();
                RepaintAvgLines();
                OnPropertyChanged(nameof(AvgLinesColor));
            }
        }

        private OxyColor _linesColor = OxyColor.FromRgb(0, 0, 0);
        public OxyColor LinesColor
        {
            get => _linesColor;
            set
            {
                _linesColor = value;
                RepaintLines();
                OnPropertyChanged(nameof(LinesColor));
            }
        }


        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "График скоростей" };
            Plot.MouseDown += PlotOnMouseDown;

            EventAggregator.Instance.CmpScanParametersChanged += OnTimeOffsetChanged;
            EventAggregator.Instance.CmpDataProcessed += OnCmpDataProcessed;
            EventAggregator.Instance.SumDataProcessed += OnSumProcessed;
            EventAggregator.Instance.SummationStarted += OnSummationStarted;
            EventAggregator.Instance.FileLoaded += OnFileLoaded;
            EventAggregator.Instance.PlotVisualOptionsChanged += OnPlotVisualOptionsChanged;
            EventAggregator.Instance.SumScanOptionsChanged += OnSumScanOptionsChanged;
            EventAggregator.Instance.CmpScanParametersChanged += OnCmpScanParametersChanged;

            _plotLoader = new PlotLoader(Plot, false, AddPalette);
            _layersLoader = new LayersLoader(Plot);
        }


        private Axis TimeAxis => Plot.Axes.First(x => x.Position == AxisPosition.Left);

        private void OnCmpDataProcessed(object obj, CmpDataProcessedEventArgs args)
        {
            Clear();
            _cmpScan = args.CmpScan;
        }

        private void RepaintAvgLines()
        {
            foreach (var line in Plot.Annotations.OfType<PolylineAnnotation>().Where(x => x.LineStyle == LineStyle.Dash))
                line.Color = AvgLinesColor;

            Plot.InvalidatePlot(true);
        }

        private void RepaintLines()
        {
            foreach (var line in Plot.Annotations.OfType<PolylineAnnotation>().Where(x => x.LineStyle == LineStyle.Solid))
                line.Color = LinesColor;
            
            Plot.InvalidatePlot(true);
        }

        private async void Sum()
        {
            _summedScan = new SummedScanVT(_cmpScan, MaxVelocity);
            await Task.Run(() =>
            {
                _summedScan.Sum(_cmpScan);
            }).ContinueWith(task =>
            {
                _plotLoader.LoadSummedScan(_summedScan, _cmpScan, _palette);
            });
            _summedScan.RefreshLayers += OnRefreshLayers;
            EventAggregator.Instance.Invoke(this, new SummationFinishedEventArgs(_summedScan));
        }

        private void OnRefreshLayers(object o, RefreshLayersEventArgs e)
        {
            _layersLoader.LoadLayers(AvgLinesColor, _summedScan, _cmpScan);
        }

        private void OnSumProcessed(object o, SumDataProcessedEventArgs e)
        {
            _summedScan = e.SumScan;
            _plotLoader.LoadSummedScan(_summedScan, _cmpScan, _palette);
            _layersLoader.LoadLayers(AvgLinesColor, _summedScan, _cmpScan);
        }

        private void PlotOnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == OxyMouseButton.Left && TimeAxis != null)
            {
                var axisX = Plot.Axes.First(x => x.Position == AxisPosition.Top);
                var axisY = Plot.Axes.First(x => x.Position == AxisPosition.Left);

                var point = Axis.InverseTransform(e.Position, axisX, axisY);
                point = new DataPoint(point.X / 100, point.Y);
                if (!IsPointOnPlot(point))
                    return;

                SelectHodograph(point);
            }
        }

        private void SelectHodograph(DataPoint point)
        {
            if (_autoCorrection)
                point = CorrectPoint(point);

            var velocity = point.X;
            var time = point.Y;
            _summedScan.AddLayer(velocity, time);

            Plot.InvalidatePlot(true);
        }

        private bool IsPointOnPlot(DataPoint point)
        {
            var velocity = point.X;
            var time = point.Y;
            if (velocity < CmpMath.Instance.WaterVelocity || velocity >= CmpMath.SpeedOfLight / 2)
                return false;
            if (time < TimeAxis.AbsoluteMinimum || time >= TimeAxis.AbsoluteMaximum)
                return false;
            return true;
        }

        private DataPoint CorrectPoint(DataPoint point)
        {
            var velocity = point.X;
            var time = point.Y;
            var correctedPoint = _summedScan.CorrectPoint(velocity, time);
            return correctedPoint != null
                ? new DataPoint(correctedPoint.Item1, correctedPoint.Item2)
                : point;
        }

        public void AddPalette(PaletteType palette)
        {
            var oxyPalette = OxyPalettes.Jet(colorsCount);
            switch (palette)
            {
                case PaletteType.Gray:
                    oxyPalette = OxyPalettes.Gray(colorsCount);
                    break;
                case PaletteType.BlackWhiteRed:
                    oxyPalette = OxyPalettes.BlackWhiteRed(colorsCount);
                    break;
                case PaletteType.BlueWhiteRed:
                    oxyPalette = OxyPalettes.BlueWhiteRed(colorsCount);
                    break;
                case PaletteType.HueDistinct:
                    oxyPalette = OxyPalettes.HueDistinct(colorsCount);
                    break;
                case PaletteType.Hue:
                    oxyPalette = OxyPalettes.Hue(colorsCount);
                    break;
                case PaletteType.Rainbow:
                    oxyPalette = OxyPalettes.Rainbow(colorsCount);
                    break;
                case PaletteType.Cool:
                    oxyPalette = OxyPalettes.Cool(colorsCount);
                    break;
                case PaletteType.Hot:
                    oxyPalette = OxyPalettes.Hot(colorsCount);
                    break;
            }
            Plot.Axes.Add(new LinearColorAxis { Palette = oxyPalette });
        }

        private void OnSummationStarted(object obj, EventArgs e)
        {
            Sum();
        }

        private void OnPlotVisualOptionsChanged(object o, PlotVisualOptionsChangedEventArgs e)
        {
            _palette = e.Palette;
            _plotLoader.Interpolation = e.Interpolation;
            AvgLinesColor = e.ShowAverageProperties
                ? OxyColor.FromArgb(e.ColorHodograph.A, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B)
                : OxyColor.FromArgb(0, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B);
            LinesColor = e.ShowLayersProperties
                ? OxyColor.FromArgb(e.ColorHodograph.A, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B)
                : OxyColor.FromArgb(0, e.ColorHodograph.R, e.ColorHodograph.G, e.ColorHodograph.B);

            if (Plot == null)
                return;
            if (Plot.Axes.Any(x => x is LinearColorAxis))
                Plot.Axes.Remove(Plot.Axes.First(x => x is LinearColorAxis));
            if (!Plot.Axes.Any(x => x is LinearColorAxis))
                AddPalette(_palette);

            var heatmap = Plot.Series.FirstOrDefault(x => x is HeatMapSeries);
            if (heatmap == null)
                return;
            (heatmap as HeatMapSeries).Interpolate = e.Interpolation;

            Plot.InvalidatePlot(true);
        }

        private void OnFileLoaded(object sender, FileLoadedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            Plot.Series.Clear();
            Plot.Axes.Clear();
            Plot.Annotations.Clear();
            Plot.InvalidatePlot(true);
        }

        private void OnSumScanOptionsChanged(object o, SumScanOptionsChangedEventArgs e)
        {
            _autoCorrection = e.AutoCorrection;
            _layersLoader.LoadLayers(AvgLinesColor, _summedScan, _cmpScan, e.Alpha);
            _halfWaveSize = e.HalfWaveLength;
            if (_summedScan != null)
                _summedScan.CheckRadius = _halfWaveSize;
        }

        private void OnCmpScanParametersChanged(object o, CmpScanParametersChangedEventArgs e)
        {
            Clear();
        }
        
        private void RepaintAvgPoints()
        {
            if (!Plot.Annotations.Any(x => x is PointAnnotation))
                return;
            foreach (var point in Plot.Annotations.Where(x => x is PointAnnotation)) (point as PointAnnotation).Fill = AvgLinesColor;

            Plot.InvalidatePlot(true);
        }

        private void OnTimeOffsetChanged(object obj, CmpScanParametersChangedEventArgs e)
        {
            Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    internal class PointsComparer<T> : IComparer<DataPoint>
    {
        public int Compare(DataPoint x, DataPoint y)
        {
            return Convert.ToInt32(2*(x.Y - y.Y));
        }
    }
}