using System;
using System.Linq;
using System.Windows.Annotations;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Annotations;

namespace SummedScanModule.ViewModels
{
    public interface ILayersLoader
    {
        void LoadLayers(OxyColor avgLinesColor, ISummedScanVT summedScan, ICmpScan cmpScan, byte? alpha = null);
    }



    public class LayersLoader : ILayersLoader
    {
        private const double _layersStructureStrokeThickness = 1;
        private const double _avgLayersStructureStrokeThickness = 1;
        private const int _pointSize = 1;

        private ISummedScanVT _summedScan;
        private ICmpScan _cmpScan;
        private OxyColor _avgLinesColor;
        private PlotModel _plot;
        private byte _alpha = 0;


        public PolylineAnnotation SolidLine => _plot.Annotations.FirstOrDefault(x =>
            x is PolylineAnnotation && ((PolylineAnnotation) x).LineStyle == LineStyle.Solid) as PolylineAnnotation;
        public PolylineAnnotation DashedLine => _plot.Annotations.FirstOrDefault(x =>
            x is PolylineAnnotation && ((PolylineAnnotation) x).LineStyle == LineStyle.Dash) as PolylineAnnotation;


        public LayersLoader(PlotModel plot)
        {
            _plot = plot;
        }


        public void LoadLayers(OxyColor avgLinesColor, ISummedScanVT summedScan, ICmpScan cmpScan, byte? alpha = null)
        {
            _summedScan = summedScan;
            _cmpScan = cmpScan;
            _avgLinesColor = avgLinesColor;

            RemoveAnnotationsExceptRectangle();

            RefreshAvgHodographLines();
            RefreshAvgHodographPoints();
            RefreshLayerHodographLines();

            _plot.InvalidatePlot(true);
        }

        private void RemoveAnnotationsExceptRectangle()
        {
            var annotations = _plot.Annotations.Where(x => x.GetType() != typeof(RectangleAnnotation)).ToList();
            foreach (var a in annotations)
                _plot.Annotations.Remove(a);
        }

        private void RefreshLayerHodographLines()
        {
            if (_summedScan.Layers.Count == 0)
                return;

            var velocity = _summedScan.Layers[0].Item1 * 100;
            var time = _summedScan.Layers[0].Item2;
            CreateLayersLine(velocity, time);

            if (_summedScan.Layers.Count <= 1) return;
            for (int i = 1; i < _summedScan.Layers.Count; i++)
            {
                AddPointToLayersLine(i);
            }
        }

        private void AddPointToLayersLine(int i)
        {
            var avgVelocity = _summedScan.Layers[i].Item1;
            var time = _summedScan.Layers[i].Item2;
            var depth = CmpMath.Instance.Depth(avgVelocity, time);
            var prevAvgVelocity = _summedScan.Layers[i - 1].Item1;
            var prevTime = _summedScan.Layers[i - 1].Item2;
            var prevDepth = CmpMath.Instance.Depth(prevAvgVelocity, prevTime);
            var layerVelocity = Math.Round(CmpMath.Instance.LayerVelocity(time, 0, depth, avgVelocity, prevDepth, prevAvgVelocity), 3);
            layerVelocity *= 100;

            var points = SolidLine.Points;

            points.Remove(points.Last());

            var beforeNewPoint = new DataPoint(layerVelocity, points.Last().Y);
            var newPoint = new DataPoint(layerVelocity, time);
            var afterNewPoint = new DataPoint(layerVelocity, _cmpScan.MaxTime);

            points.Add(beforeNewPoint);
            points.Add(newPoint);
            points.Add(afterNewPoint);
        }

        private void CreateLayersLine(double velocity, double time)
        {
            var layersStructure = new PolylineAnnotation
            {
                Color = _avgLinesColor,
                LineStyle = LineStyle.Solid,
                StrokeThickness = _layersStructureStrokeThickness
            };

            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MinTime));
            layersStructure.Points.Add(new DataPoint(velocity, time));
            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MaxTime));

            _plot.Annotations.Add(layersStructure);
        }

        private void RefreshAvgHodographLines()
        {
            if (_summedScan.Layers.Count == 0)
                return;

            var velocity = _summedScan.Layers[0].Item1 * 100;
            var time = _summedScan.Layers[0].Item2;
            CreateAvgLine(velocity, time);

            if (_summedScan.Layers.Count <= 1) return;
            for (int i = 1; i < _summedScan.Layers.Count; i++)
            {
                velocity = _summedScan.Layers[i].Item1 * 100;
                time = _summedScan.Layers[i].Item2;
                AddPointToAvgLine(velocity, time);
            }
        }

        private void CreateAvgLine(double velocity, double time)
        {
            var layersStructure = new PolylineAnnotation
            {
                Color = _avgLinesColor,
                LineStyle = LineStyle.Dash,
                StrokeThickness = _avgLayersStructureStrokeThickness
            };

            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MinTime));
            layersStructure.Points.Add(new DataPoint(velocity, time));
            layersStructure.Points.Add(new DataPoint(velocity, _cmpScan.MaxTime));

            _plot.Annotations.Add(layersStructure);
        }

        private void AddPointToAvgLine(double velocity, double time)
        {
            var points = DashedLine.Points;

            points.Remove(points.Last());

            var beforeNewPoint = new DataPoint(velocity, points.Last().Y);
            var newPoint = new DataPoint(velocity, time);
            var afterNewPoint = new DataPoint(velocity, _cmpScan.MaxTime);

            points.Add(beforeNewPoint);
            points.Add(newPoint);
            points.Add(afterNewPoint);
        }

        private void RefreshAvgHodographPoints()
        {
            foreach (var layer in _summedScan.Layers)
            {
                var velocity = layer.Item1 * 100;
                var time = layer.Item2;
                var point = new PointAnnotation();
                point.X = velocity;
                point.Y = time;
                point.Size = _pointSize;
                point.Fill = _avgLinesColor;
                _plot.Annotations.Add(point);
            }
        }
    }
}