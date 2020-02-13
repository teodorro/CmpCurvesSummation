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
        event SummationFinishedHandler SummationFinished;
        void OnCmpDataProcessed(object obj, CmpProcessedEventArgs args);
        void AddPalette(PaletteType palette);
        void OnSummationStarted(object obj, EventArgs e);
        void OnPaletteChanged(object obj, PaletteChangedEventArgs e);
        void OnFileLoaded(object sender, FileLoadedEventArgs e);
        void OnAutoCorrectionChange(object sender, AutoCorrectionCheckEventArgs e);
        void OnStepDistanceChanged(object obj, StepDistanceEventArgs e);
        void OnStepTimeChanged(object obj, StepTimeEventArgs e);
        void OnPointColorChanged(object obj, PointColorChangedEventArgs e);
    }



    public class SummedScanViewModel : ISummedScanViewModel, INotifyPropertyChanged
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        private PaletteType _palette = PaletteType.Jet;
        private int _halfWaveSize = 5;
        private IPlotLoader _plotLoader;
        private ILayersLoader _layersLoader;

        public PlotModel Plot { get; }
        public double MaxVelocity { get; private set; } = CmpMath.PlotMaxVelocity;

        private bool _autoCorrection;
        
        private OxyColor _avgLinesColor = OxyColor.FromRgb(255, 255, 255);
        public OxyColor AvgLinesColor
        {
            get => _avgLinesColor;
            set
            {
                _avgLinesColor = value;
                RepaintPoints();
                RepaintLines();
                OnPropertyChanged(nameof(AvgLinesColor));
            }
        }


        public event SummationFinishedHandler SummationFinished;


        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "После суммирования" };
            Plot.MouseDown += PlotOnMouseDown;

            _plotLoader = new PlotLoader(Plot, false, AddPalette);
            _layersLoader = new LayersLoader(Plot);
        }


        private Axis TimeAxis => Plot.Axes.First(x => x.Position == AxisPosition.Left);

        public void OnCmpDataProcessed(object obj, CmpProcessedEventArgs args)
        {
            Clear();
            _cmpScan = args.CmpScan;
        }

        private void RepaintLines()
        {
            foreach (var line in Plot.Annotations.OfType<PolylineAnnotation>())
                line.Color = AvgLinesColor;
            
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
            SummationFinished?.Invoke(this, new SummationFinishedEventArgs(_summedScan));
        }

        private void OnRefreshLayers(object o, RefreshLayersEventArgs e)
        {
            _layersLoader.LoadLayers(AvgLinesColor, _summedScan, _cmpScan);
        }

        public void OnSumProcessed(object o, SumProcessedEventArgs e)
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
            Clear();
        }

        private void Clear()
        {
            Plot.Series.Clear();
            Plot.Axes.Clear();
            Plot.Annotations.Clear();
            Plot.InvalidatePlot(true);
        }

        public void OnAutoCorrectionChange(object sender, AutoCorrectionCheckEventArgs e)
        {
            _autoCorrection = e.Auto;
        }
        
        public void OnStepDistanceChanged(object obj, StepDistanceEventArgs e)
        {
            Clear();
            _cmpScan = e.CmpScan;
        }

        public void OnStepTimeChanged(object obj, StepTimeEventArgs e)
        {
            Clear();
            _cmpScan = e.CmpScan;
        }

        public void OnPointColorChanged(object obj, PointColorChangedEventArgs e)
        {
            AvgLinesColor = OxyColor.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
        }
        
        private void RepaintPoints()
        {
            if (!Plot.Annotations.Any(x => x is PointAnnotation))
                return;
            foreach (var point in Plot.Annotations.Where(x => x is PointAnnotation))
            {
                (point as PointAnnotation).Fill = AvgLinesColor;
            }

            Plot.InvalidatePlot(true);
        }

        public void OnTimeOffsetChanged(object obj, TimeOffsetChangedEventArgs e)
        {
            Clear();
            _cmpScan = e.CmpScan;
        }

        public void OnInterpolationChanged(object obj, InterpolationChangedEventArgs e)
        {
            if (Plot == null)
                return;
            var heatmap = Plot.Series.FirstOrDefault(x => x is HeatMapSeries);
            if (heatmap == null)
                return;
            _plotLoader.Interpolation = e.Interpolation;
            (heatmap as HeatMapSeries).Interpolate = e.Interpolation;
            Plot.InvalidatePlot(true);
        }

        public void OnAlphaChanged(object obj, AlphaChangedEventArgs e)
        {
            _layersLoader.LoadLayers(AvgLinesColor, _summedScan, _cmpScan, e.Alpha);
        }

        public void OnHalfWaveSizeChanged(object obj, HalfWaveSizeChangedEventArgs e)
        {
            _halfWaveSize = e.HalfWaveSize;
            if (_summedScan != null)
                _summedScan.CheckRadius = _halfWaveSize;
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