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

        public double Velocity { get; set; }
        public double Time { get; set; }
    }


    public class DeleteLayerEventArgs : EventArgs
    {
        public DeleteLayerEventArgs(double velocity, double time)
        {
            Velocity = velocity;
            Time = time;
        }

        public double Velocity { get; set; }
        public double Time { get; set; }
    }


    public class AutoSummationCheckEventArgs : EventArgs
    {
        public AutoSummationCheckEventArgs(bool auto)
        {
            Auto = auto;
        }

        public bool Auto { get; set; }
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

        public PaletteType Palette { get; set; }
    }

}