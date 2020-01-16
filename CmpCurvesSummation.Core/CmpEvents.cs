using System;

namespace CmpCurvesSummation.Core
{

    public enum PaletteType
    {
        Jet,
        Gray,
        BW
    }



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


    public class SummationClickEventArgs : EventArgs
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













}