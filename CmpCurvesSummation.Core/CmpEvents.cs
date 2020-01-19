using System;

namespace CmpCurvesSummation.Core
{

    public enum PaletteType
    {
        Jet,
        Gray,
        BW
    }


    public delegate void HodographDrawClickHander(object obj, HodographDrawVTClickEventArgs e);
    public delegate void EndSummationHandler(object obj, SummationFinishedEventArgs e);
    public delegate void AutoSummationCheckHander(object obj, AutoSummationCheckEventArgs e);
    public delegate void SummationStartedHander(object obj, SummationStartedClickEventArgs e);
    public delegate void PaletteChangedHander(object obj, PaletteChangedEventArgs e);
    public delegate void StepDistanceChangedHandler(object obj, StepDistanceEventArgs e);
    public delegate void StepTimeChangedHandler(object obj, StepTimeEventArgs e);
    public delegate void FileOpenHandler(object obj, FileLoadedEventArgs e);
    public delegate void DeleteLayerHander(object obj, DeleteLayerEventArgs e);
    public delegate void RawCmpProcessedHandler(object obj, RawCmpProcessedEventArgs e);
    public delegate void SummationInProcessHandler(object obj, SummationInProcessEventArgs e);
    public delegate void AutoCorrectionCheckHander(object obj, AutoCorrectionCheckEventArgs e);




    public class FileLoadedEventArgs : EventArgs
    {
        public ICmpScan CmpScan { get; }
        public FileLoadedEventArgs(ICmpScan cmpScan)
        {
            CmpScan = cmpScan;
        }
    }


    public class RawCmpProcessedEventArgs : EventArgs
    {
        public ICmpScan CmpScan { get; }
        public RawCmpProcessedEventArgs(ICmpScan cmpScan)
        {
            CmpScan = cmpScan;
        }
    }
    

    public class SummedOverHodographEventArgs : EventArgs
    {
        public ISummedScanVH SummedScan { get; }
        public SummedOverHodographEventArgs(ISummedScanVH summedScan)
        {
            SummedScan = summedScan;
        }
    }


    public class HodographDrawVHClickEventArgs : EventArgs
    {
        public HodographDrawVHClickEventArgs(double velocity, double height)
        {
            Velocity = velocity;
            Height = height;
        }

        public double Velocity { get; }
        public double Height { get; }
    }


    public class HodographDrawVTClickEventArgs : EventArgs
    {
        public HodographDrawVTClickEventArgs(double velocity, double time)
        {
            Velocity = velocity;
            Time = time;
        }

        public double Velocity { get; }
        public double Time { get;  }
    }


    public class DeleteLayerEventArgs : EventArgs
    {
        public DeleteLayerEventArgs(double velocity, double time)
        {
            Velocity = velocity;
            Time = time;
        }

        public double Velocity { get; }
        public double Time { get; }
    }


    public class AutoSummationCheckEventArgs : EventArgs
    {
        public AutoSummationCheckEventArgs(bool auto)
        {
            Auto = auto;
        }

        public bool Auto { get; }
    }


    public class SummationStartedClickEventArgs : EventArgs
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
    }


    public class PaletteChangedEventArgs : EventArgs
    {
        public PaletteChangedEventArgs(PaletteType palette)
        {
            Palette = palette;
        }

        public PaletteType Palette { get; }
    }


    public class StepDistanceEventArgs : EventArgs
    {
        public StepDistanceEventArgs(double newStepDistance, double oldStepDistance)
        {
            NewStepDistance = newStepDistance;
            OldStepDistance = oldStepDistance;
        }

        public double NewStepDistance { get; }
        public double OldStepDistance { get; }
    }


    public class StepTimeEventArgs : EventArgs
    {
        public StepTimeEventArgs(double newStepTime, double oldStepTime)
        {
            NewStepTime = newStepTime;
            OldStepTime = oldStepTime;
        }

        public double NewStepTime { get; }
        public double OldStepTime { get; }
    }


    public class AutoCorrectionCheckEventArgs : EventArgs
    {
        public AutoCorrectionCheckEventArgs(bool auto)
        {
            Auto = auto;
        }

        public bool Auto { get; }
    }





    public class CmpProgressBar
    {
        private static Lazy<CmpProgressBar> _instance = new Lazy<CmpProgressBar>(() => new CmpProgressBar());
        public static CmpProgressBar Instance => _instance.Value;

        public event SummationInProcessHandler SummationInProcess;

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                SummationInProcess.Invoke(this, new SummationInProcessEventArgs(value));
            }
        }
    }








}