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
        OxyColor PointColor { get; set; }
        event EndSummationHandler SummationFinished;
        void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args);
        void AddPalette(PaletteType palette);
        void OnSummationStarted(object obj, EventArgs e);
        void OnPaletteChanged(object obj, PaletteChangedEventArgs e);
        void OnFileLoaded(object sender, FileLoadedEventArgs e);
        void OnAutoCorrectionChange(object sender, AutoCorrectionCheckEventArgs e);
        void OnStepDistanceChanged(object obj, StepDistanceEventArgs e);
        void OnStepTimeChanged(object obj, StepTimeEventArgs e);
        void OnPointColorChanged(object obj, PointColorChangedEventArgs e);
    }



    public class SummedScanViewModel : INotifyPropertyChanged
    {
        private const int colorsCount = 1024;
        private const double _layersStructureStrokeThickness = 0.5;
        private const int _pointSize = 1;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        private PaletteType _palette = PaletteType.Jet;
        private byte _alpha = 0;
        private int _halfWaveSize = 5;
        
        public PlotModel Plot { get; }
        public double MaxVelocity { get; private set; } = CmpMath.SpeedOfLight / 2;

        private bool _autoCorrection;
        
        private OxyColor _pointColor = OxyColor.FromRgb(255, 255, 255);
        private bool _interpolation;

        public OxyColor PointColor
        {
            get => _pointColor;
            set
            {
                _pointColor = value;
                RepaintPoints();
                RepaintLine();
                OnPropertyChanged(nameof(PointColor));
            }
        }


        public event EndSummationHandler SummationFinished;


        public SummedScanViewModel()
        {
            Plot = new PlotModel { Title = "После суммирования" };

            Plot.MouseDown += PlotOnMouseDown;
        }


        private Axis TimeAxis => Plot.Axes.First(x => x.Position == AxisPosition.Left);

        public void OnRawCmpDataProcessed(object obj, RawCmpProcessedEventArgs args)
        {
            Clear();
            _cmpScan = args.CmpScan;
        }

        private void RepaintLine()
        {
            var line = Plot.Annotations.FirstOrDefault(x => x is PolylineAnnotation);
            if (line == null)
                return;
            ((PolylineAnnotation)line).Color = PointColor;
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
                LoadSummedScan();
            });
            _summedScan.RefreshLayers += OnRefreshLayers;
            SummationFinished?.Invoke(this, new SummationFinishedEventArgs(_summedScan));
        }

        private void OnRefreshLayers(object o, RefreshLayersEventArgs e)
        {
            Plot.Annotations.Clear();
            AddAlpha();
            RefreshHodographLines();
            RefreshHodographPoints();

            Plot.InvalidatePlot(true);
        }

        private void LoadSummedScan()
        {
            LoadSeries();
            UpdateAxes();
            AddAlpha();
            
            Plot.InvalidatePlot(true); 
        }

        private void AddAlpha()
        {
            var c = OxyColor.FromArgb(_alpha, 255, 255, 255);
            var rect = new RectangleAnnotation
            {
                MaximumX = _summedScan.MaxVelocity,
                MinimumX = _summedScan.MinVelocity,
                MinimumY = _summedScan.MinTime,
                MaximumY = _summedScan.MaxTime,
                Fill = c
            };
            Plot.Annotations.Add(rect);
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
                if (!IsPointOnPlot(point))
                    return;

                SelectHodograph(point);
            }
        }

        private void SelectHodograph(DataPoint point)
        {
            if (_autoCorrection)
                point = CorrectPoint(point);
            
            var velocity = Math.Round(point.X, 3);
            var time = Math.Round(point.Y, 2);
            AddHodographPoint(velocity, time);

            Plot.InvalidatePlot(true);

        }

        private void AddHodographPoint(double velocity, double time)
        {
            _summedScan.AddLayer(velocity, time);
        }
        
        private void RefreshHodographPoints()
        {
            foreach (var layer in _summedScan.Layers)
            {
                var velocity = layer.Item1;
                var time = layer.Item2;
                var point = new PointAnnotation();
                point.X = velocity;
                point.Y = time;
                point.Size = _pointSize;
                point.Fill = PointColor;
                Plot.Annotations.Add(point);
            }
        }

        private void RefreshHodographLines()
        {
            if (_summedScan.Layers.Count == 0)
                return;
            
            var velocity = _summedScan.Layers[0].Item1;
            var time = _summedScan.Layers[0].Item2;
            CreateLayersLine(velocity, time);

            if (_summedScan.Layers.Count > 1)
            {
                for (int i = 1; i < _summedScan.Layers.Count; i++)
                {
                    velocity = _summedScan.Layers[i].Item1;
                    time = _summedScan.Layers[i].Item2;
                    AddPointToLayersLine(velocity, time);
                }
            }
        }

        private void CreateLayersLine(double velocity, double time)
        {
            var layersStructure = new PolylineAnnotation();
            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MinTime));
            layersStructure.Points.Add(new DataPoint(velocity, time));
            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MaxTime));

            layersStructure.Color = PointColor;
            layersStructure.LineStyle = LineStyle.Solid;
            layersStructure.StrokeThickness = _layersStructureStrokeThickness;
            Plot.Annotations.Add(layersStructure);
        }

        private void AddPointToLayersLine(double velocity, double time)
        {
            var layersStructure = Plot.Annotations.FirstOrDefault(x => x is PolylineAnnotation) as PolylineAnnotation;
            var points = layersStructure.Points;

            points.Remove(points.Last());

            var beforeNewPoint = new DataPoint(velocity, points.Last().Y);
            var newPoint = new DataPoint(velocity, time);
            var afterNewPoint = new DataPoint(velocity, _cmpScan.MaxTime);

            points.Add(beforeNewPoint);
            points.Add(newPoint);
            points.Add(afterNewPoint);
        }

        private DataPoint CorrectPoint(DataPoint point)
        {
            var velocity = Math.Round(point.X, 3);
            var time = Math.Round(point.Y, 2);
            var correctedPoint = _summedScan.CorrectPoint(velocity, time);
            return correctedPoint != null 
                ? new DataPoint(correctedPoint.Item1, correctedPoint.Item2) 
                : point;
        }

        private bool IsPointOnPlot(DataPoint point)
        {
            var v = Math.Round(point.X, 3);
            var t = Math.Round(point.Y, 2);
            if (v < CmpMath.Instance.WaterVelocity || v >= CmpMath.SpeedOfLight / 2)
                return false;
            if (t < TimeAxis.AbsoluteMinimum || t >= TimeAxis.AbsoluteMaximum)
                return false;
            return true;
        }

        private void UpdateAxes()
        {
            Plot.InvalidatePlot(true); // to create axes

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
            top.AbsoluteMinimum = _summedScan.MinVelocity;
            top.AbsoluteMaximum = _summedScan.MaxVelocity;
            top.Title = "V";
            top.TitleFontSize = 1;
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

        private void LoadSeries()
        {
            Plot.Series.Clear();

            if (_summedScan != null)
            {
                var heatMapSeries = new HeatMapSeries
                {
                    X0 = _summedScan.MinVelocity,
                    X1 = _summedScan.MaxVelocity,
                    Y0 = _summedScan.MinTime,
                    Y1 = _summedScan.MaxTime,
                    Interpolate = _interpolation,
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
            PointColor = OxyColor.FromArgb(e.NewColor.A, e.NewColor.R, e.NewColor.G, e.NewColor.B);
        }
        
        private void RepaintPoints()
        {
            if (!Plot.Annotations.Any(x => x is PointAnnotation))
                return;
            foreach (var point in Plot.Annotations.Where(x => x is PointAnnotation))
            {
                (point as PointAnnotation).Fill = PointColor;
            }

            Plot.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            _interpolation = e.Interpolation;
            (heatmap as HeatMapSeries).Interpolate = e.Interpolation;
            Plot.InvalidatePlot(true);
        }

        public void OnAlphaChanged(object obj, AlphaChangedEventArgs e)
        {
            _alpha = e.Alpha;
            OnRefreshLayers(this, null);
        }

        public void OnHalfWaveSizeChanged(object obj, HalfWaveSizeChangedEventArgs e)
        {
            _halfWaveSize = e.HalfWaveSize;
            if (_summedScan != null)
                _summedScan.CheckRadius = _halfWaveSize;
        }

        public void OnMaxVelocityChanged(object obj, MaxVelocityChangedEventArgs e)
        {
            MaxVelocity = e.MaxVelocity;
            _summedScan.RemoveRightAscans(MaxVelocity);
            Clear();
            LoadSummedScan();
            OnRefreshLayers(this, null);
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