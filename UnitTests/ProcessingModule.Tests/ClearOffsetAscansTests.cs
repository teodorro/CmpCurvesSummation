using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
using ProcessingModule.Processing.CmpScan;
using Xunit;

namespace ProcessingModule.Tests
{
    public class ClearOffsetAscansTests
    {
        public ICmpScan GetSimpleCmpScan()
        {
            var a = 3;
            var cmpScan = new CmpScan();
            for (int i = 0; i < a; i++)
            {
                var ascan = new double[2 * a];
                cmpScan.RawData.Add(ascan);
                ascan[0] = 1;
            }

            return cmpScan;
        }

        [Fact]
        public void ClearAscans()
        {
            var cmpScan = GetSimpleCmpScan();
            cmpScan.CopyRawDataToProcessed();
            var processing = new ClearOffsetAscans(0);
            processing.NumberOfAscans = 2;

            processing.Process(cmpScan);

            Assert.Equal(0, cmpScan.Data[0][0]);
            Assert.Equal(0, cmpScan.Data[1][0]);
            Assert.Equal(1, cmpScan.Data[2][0]);
        }
    }
}