using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CmpCurvesSummation.Core
{
    public delegate void FileLoadedHandler(object obj, FileLoadedEventArgs e);

    public delegate void CmpDataProcessedHandler(object obj, CmpDataProcessedEventArgs e);

    public delegate void PlotVisualOptionsChangedHandler(object obj, PlotVisualOptionsChangedEventArgs e);

    public delegate void CmpScanParametersChangedHandler(object obj, CmpScanParametersChangedEventArgs e);

    public delegate void SummationFinishedHandler(object obj, SummationFinishedEventArgs e);

    public delegate void SummationStartedHandler(object obj, SummationStartedEventArgs e);

    public delegate void SummationInProcessHandler(object obj, SummationInProcessEventArgs e);

    public delegate void SumDataProcessedHandler(object obj, SumDataProcessedEventArgs e);

    public delegate void SumScanOptionsChangedHandler(object obj, SumScanOptionsChangedEventArgs e);

    public delegate void RefreshLayersHandler(object obj, RefreshLayersEventArgs e);


    public interface IEventAggregator
    {
        void Invoke(object obj, EventArgs e);
        event FileLoadedHandler FileLoaded;
        event CmpDataProcessedHandler CmpDataProcessed;
        event PlotVisualOptionsChangedHandler PlotVisualOptionsChanged;
        event CmpScanParametersChangedHandler CmpScanParametersChanged;
        event SummationStartedHandler SummationStarted;
        event SummationInProcessHandler SummationInProcess;
        event SummationFinishedHandler SummationFinished;
        event SumDataProcessedHandler SumDataProcessed;
        event SumScanOptionsChangedHandler SumScanOptionsChanged;
    }


    public class EventAggregator : IEventAggregator
    {
        private static readonly Lazy<IEventAggregator> _instance = new Lazy<IEventAggregator>(() => new EventAggregator());
        public static IEventAggregator Instance => _instance.Value;

        public event FileLoadedHandler FileLoaded;
        public event CmpDataProcessedHandler CmpDataProcessed;
        public event PlotVisualOptionsChangedHandler PlotVisualOptionsChanged;
        public event CmpScanParametersChangedHandler CmpScanParametersChanged;
        public event SummationStartedHandler SummationStarted;
        public event SummationInProcessHandler SummationInProcess;
        public event SummationFinishedHandler SummationFinished;
        public event SumDataProcessedHandler SumDataProcessed;
        public event SumScanOptionsChangedHandler SumScanOptionsChanged;

        public void Invoke(object obj, EventArgs e)
        {
            switch (e)
            {
                case FileLoadedEventArgs args:
                    FileLoaded?.Invoke(obj, args);
                    break;
                case SummationInProcessEventArgs args:
                    SummationInProcess?.Invoke(obj, args);
                    break;
                case CmpDataProcessedEventArgs args:
                    CmpDataProcessed?.Invoke(obj, args);
                    break;
                case PlotVisualOptionsChangedEventArgs args:
                    PlotVisualOptionsChanged?.Invoke(obj, args);
                    break;
                case CmpScanParametersChangedEventArgs args:
                    CmpScanParametersChanged?.Invoke(obj, args);
                    break;
                case SummationStartedEventArgs args:
                    SummationStarted?.Invoke(obj, args);
                    break;
                case SummationFinishedEventArgs args:
                    SummationFinished?.Invoke(obj, args);
                    break;
                case SumDataProcessedEventArgs args:
                    SumDataProcessed?.Invoke(obj, args);
                    break;
                case SumScanOptionsChangedEventArgs args:
                    SumScanOptionsChanged?.Invoke(obj, args);
                    break;
            }
        }
    }


    public class FileLoadedEventArgs : EventArgs
    {
        public ICmpScan CmpScan { get; }
        public string Filename { get; }

        public FileLoadedEventArgs(ICmpScan cmpScan, string filename)
        {
            CmpScan = cmpScan;
            Filename = filename;
        }
    }

    public class CmpDataProcessedEventArgs : EventArgs
    {
        public ICmpScan CmpScan { get; }

        public CmpDataProcessedEventArgs(ICmpScan cmpScan)
        {
            CmpScan = cmpScan;
        }
    }

    public class SummationStartedEventArgs : EventArgs
    {
    }

    public class SummationInProcessEventArgs : EventArgs
    {
        public SummationInProcessEventArgs(int percent)
        {
            Percent = percent;
        }

        public int Percent { get; }
    }

    public class SummationFinishedEventArgs : EventArgs
    {
        public SummationFinishedEventArgs(ISummedScanVT summedScan)
        {
            SummedScan = summedScan;
        }

        public ISummedScanVT SummedScan { get; }
    }

    public class SumDataProcessedEventArgs : EventArgs
    {
        public ISummedScanVT SumScan { get; }

        public SumDataProcessedEventArgs(ISummedScanVT sumScan)
        {
            SumScan = sumScan;
        }
    }

    public class CmpScanParametersChangedEventArgs : EventArgs
    {
        public double OldStepDistance { get; }
        public double OldStepTime { get; }
        public double OldTimeOffset { get; }
        public double StepDistance { get; }
        public double StepTime { get; }
        public double TimeOffset { get; }

        public CmpScanParametersChangedEventArgs(double oldStepDistance, double oldStepTime, double oldTimeOffset, double stepDistance, double stepTime, double timeOffset)
        {
            OldStepDistance = oldStepDistance;
            OldStepTime = oldStepTime;
            OldTimeOffset = oldTimeOffset;
            StepDistance = stepDistance;
            StepTime = stepTime;
            TimeOffset = timeOffset;
        }
    }

    public class PlotVisualOptionsChangedEventArgs : EventArgs
    {
        public PaletteType Palette { get; }
        public Color ColorHodograph { get; }
        public Color ColorLayerLine { get; }
        public bool Interpolation { get; }

        public PlotVisualOptionsChangedEventArgs(PaletteType palette, Color colorHodograph, Color colorLayerLine, bool interpolation)
        {
            Palette = palette;
            ColorHodograph = colorHodograph;
            ColorLayerLine = colorLayerLine;
            Interpolation = interpolation;
        }
    }

    public class SumScanOptionsChangedEventArgs : EventArgs
    {
        public bool AutoCorrection { get; }
        public byte Alpha { get; }
        public int HalfWaveLength { get; }

        public SumScanOptionsChangedEventArgs(bool autoCorrection, byte alpha, int halfWaveLength)
        {
            AutoCorrection = autoCorrection;
            Alpha = alpha;
            HalfWaveLength = halfWaveLength;
        }
    }

    public class RefreshLayersEventArgs : EventArgs
    {
        public RefreshLayersEventArgs(IEnumerable<Tuple<double, double>> layers)
        {
            Layers = layers;
        }

        public IEnumerable<Tuple<double, double>> Layers { get; }
    }
}