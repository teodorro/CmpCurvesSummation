using System;

namespace CmpCurvesSummation.Core
{
    

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

    public class DeleteLayerEventsArgs : EventArgs
    {
        public DeleteLayerEventsArgs(double velocity, double time)
        {
            Velocity = velocity;
            Time = time;
        }

        public double Velocity { get; set; }
        public double Time { get; set; }
    }



}