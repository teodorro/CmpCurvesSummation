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

    public delegate void CmpProcessingListChangedHandler(object obj, CmpProcessingListChangedEventArgs e);

    public delegate void CmpProcessingValuesChangedHandler(object obj, CmpProcessingValuesChangedEventArgs e);

    public delegate void SumProcessingListChangedHandler(object obj, SumProcessingListChangedEventArgs e);

    public delegate void SumProcessingValuesChangedHandler(object obj, SumProcessingValuesChangedEventArgs e);


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
        event CmpProcessingListChangedHandler CmpProcessingListChanged;
        event CmpProcessingValuesChangedHandler CmpProcessingValuesChanged;
        event SumProcessingListChangedHandler SumProcessingListChanged;
        event SumProcessingValuesChangedHandler SumProcessingValuesChanged;
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
        public event CmpProcessingListChangedHandler CmpProcessingListChanged;
        public event CmpProcessingValuesChangedHandler CmpProcessingValuesChanged;
        public event SumProcessingListChangedHandler SumProcessingListChanged;
        public event SumProcessingValuesChangedHandler SumProcessingValuesChanged;

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
                case CmpProcessingListChangedEventArgs args:
                    CmpProcessingListChanged?.Invoke(obj, args);
                    break;
                case CmpProcessingValuesChangedEventArgs args:
                    CmpProcessingValuesChanged?.Invoke(obj, args);
                    break;
                case SumProcessingListChangedEventArgs args:
                    SumProcessingListChanged?.Invoke(obj, args);
                    break;
                case SumProcessingValuesChangedEventArgs args:
                    SumProcessingValuesChanged?.Invoke(obj, args);
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
        public bool Interpolation { get; }
        public int Alpha { get; }
        public bool ShowHodographs { get; }
        public bool ShowLayersProperties { get; }
        public bool ShowAverageProperties { get; }
        public Color ColorHodograph { get; }
        public Color ColorLayerLine { get; }

        public PlotVisualOptionsChangedEventArgs(PaletteType palette, bool interpolation, int alpha,
            bool showHodographs, bool showLayersProperties, bool showAverageProperties, Color colorHodograph, Color colorLayerLine)
        {
            Palette = palette;
            Interpolation = interpolation;
            Alpha = alpha;

            ShowHodographs = showHodographs;
            ShowLayersProperties = showLayersProperties;
            ShowAverageProperties = showAverageProperties;
            ColorHodograph = colorHodograph;
            ColorLayerLine = colorLayerLine;
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

    public class CmpProcessingListChangedEventArgs : EventArgs
    {
        public object Processing { get; set; }
        public bool? Enabled { get; set; }
    }

    public class CmpProcessingValuesChangedEventArgs : EventArgs
    {
    }

    public class SumProcessingListChangedEventArgs : EventArgs
    {
        public object Processing { get; set; }
        public bool? Enabled { get; set; }
    }

    public class SumProcessingValuesChangedEventArgs : EventArgs
    {
    }
}