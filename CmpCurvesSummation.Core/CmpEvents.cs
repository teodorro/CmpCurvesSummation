﻿using System;

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
        public ISummedScan SummedScan { get; }
        public SummedOverHodographEventArgs(ISummedScan summedScan)
        {
            SummedScan = summedScan;
        }
    }

    public class HodographDrawClickEventArgs : EventArgs
    {
        public double V { get; set; }
        public double H { get; set; }
    }




}