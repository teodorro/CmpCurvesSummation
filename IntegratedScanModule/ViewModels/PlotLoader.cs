

using System;
using System.Linq;
using CmpCurvesSummation.Core;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SummedScanModule.ViewModels
{
    public interface IPlotLoader
    {
        void LoadSummedScan(ISummedScanVT summedScan, ICmpScan cmpScan, PaletteType palette);
        bool Interpolation { get; set; }
    }


    public class PlotLoader : IPlotLoader
    {
        private PlotModel _plot;
        private ISummedScanVT _summedScan;
        private ICmpScan _cmpScan;
        private PaletteType _palette;
        private readonly Action<PaletteType> AddPalette;

        public bool Interpolation { get; set; }


        public PlotLoader(PlotModel plot, bool interpolation, Action<PaletteType> addPalette)
        {
            _plot = plot;
            Interpolation = interpolation;
            AddPalette = addPalette;
        }


        public void LoadSummedScan(ISummedScanVT summedScan, ICmpScan cmpScan, PaletteType palette)
        {
            _summedScan = summedScan;
            _cmpScan = cmpScan;
            _palette = palette;

            LoadSeries();
            UpdateAxes();

            _plot.InvalidatePlot(true);
        }

        private void LoadSeries()
        {
            _plot.Series.Clear();

            if (_summedScan != null)
            {
                var heatMapSeries = new HeatMapSeries
                {
                    X0 = _summedScan.MinVelocity,
                    X1 = _summedScan.MaxVelocity,
                    Y0 = _summedScan.MinTime,
                    Y1 = _summedScan.MaxTime,
                    Interpolate = Interpolation,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = _summedScan.GetDataArray()
                };
                _plot.Series.Add(heatMapSeries);
            }
            else
            {
                _plot.Series.Add(new HeatMapSeries());
            }
        }

        private void UpdateAxes()
        {
            _plot.InvalidatePlot(true); // to create axes

            if (!_plot.Axes.Any(x => x is LinearColorAxis))
                AddPalette(_palette);

            if (_plot.Axes.Count == 1)
                return;

            TuneHorizontalAxis();
            TuneVerticalAxis(_cmpScan.MinTime, _cmpScan.MaxTime);

            _plot.InvalidatePlot(true); // to update axes in UI
        }

        private void TuneVerticalAxis(double min, double max)
        {
            var left = _plot.Axes.First(x => x.Position == AxisPosition.Left);
            left.StartPosition = 1;
            left.EndPosition = 0;
            left.AbsoluteMinimum = min;
            left.AbsoluteMaximum = max;
            left.Title = "T";
            left.TitleFontSize = 1;
        }

        private void TuneHorizontalAxis()
        {
            if (_plot.Axes.All(x => x.Position != AxisPosition.Top))
                _plot.Axes.First(x => x.Position == AxisPosition.Bottom).Position = AxisPosition.Top;
            var top = _plot.Axes.First(x => x.Position == AxisPosition.Top);
            top.AbsoluteMinimum = _summedScan.MinVelocity;
            top.AbsoluteMaximum = _summedScan.MaxVelocity;
            top.Title = "V";
            top.TitleFontSize = 1;
        }
    }
}