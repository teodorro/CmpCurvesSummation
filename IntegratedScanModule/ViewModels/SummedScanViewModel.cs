using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public class SummedScanViewModel
    {
        private const int colorsCount = 1024;

        private ICmpScan _cmpScan;
        private ISummedScanVT _summedScan;
        private PaletteType _palette = PaletteType.Jet;
        

        public PlotModel Plot { get; }
        public bool AutoSummation { get; set; }
        private bool _autoCorrection { get; set; }


        public event HodographDrawClickHander HodographDrawClick;
        public event EndSummationHandler SummationFinished;
        public event DeleteLayerHander DeleteClick;


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
            
            Plot.InvalidatePlot(true); 
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
            {
                var correctedPoint = CorrectPoint(point);
                RemovePointsWithCloseTime(point);
                AddHodographToPlot(correctedPoint.X, correctedPoint.Y);
            }
            else
            {
                RemovePointsWithCloseTime(point);
                var velocity = Math.Round(point.X, 3);
                var time = Math.Round(point.Y, 2);
                AddHodographToPlot(velocity, time);
            }
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

        private void RemovePointsWithCloseTime(DataPoint newPoint)
        {
            var points = Plot.Annotations.OfType<PointAnnotation>();
            if (points == null || !points.Any())
                return;
            var pointsToRemove = new List<PointAnnotation>();
            foreach (var point in points)
                if (Math.Abs(Math.Abs(point.Y) - Math.Abs(newPoint.Y)) < _summedScan.CheckRadius)
                    pointsToRemove.Add(point);
            if (pointsToRemove.Any())
            {
                foreach (var point in pointsToRemove)
                {
                    Plot.Annotations.Remove(point);
                    DeleteClick?.Invoke(this, new DeleteLayerEventArgs(point.X, point.Y));
                }
            }
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





    }
}