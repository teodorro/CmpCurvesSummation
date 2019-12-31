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


    public class SummedOverHodographEventArgs : EventArgs
    {
        public ISummedOverHodographScan SummedOverHodographScan { get; }
        public SummedOverHodographEventArgs(ISummedOverHodographScan summedOverHodographScan)
        {
            SummedOverHodographScan = summedOverHodographScan;
        }
    }






}