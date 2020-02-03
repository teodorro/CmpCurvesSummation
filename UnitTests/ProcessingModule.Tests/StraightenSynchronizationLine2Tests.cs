using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
using Xunit;

namespace ProcessingModule.Tests
{
    public class StraightenSynchronizationLine2Tests
    {
        public ICmpScan GetCmpScan1()
        {
            var cmpScan = new CmpScan();
            var ascan1 = new double[] { 0, 2, 4, 5, 0, 0, -2, -1 };
            var ascan2 = new double[] { 0, 4, 0, 1, -1, 0, -3, -4 };
            var ascan3 = new double[] { 0, 2, 2, 2, -7, -8, -7, -5 };
            cmpScan.RawData.Add(ascan1);
            cmpScan.RawData.Add(ascan2);
            cmpScan.RawData.Add(ascan3);

            return cmpScan;
        }


        [Fact]
        public void AscansMove()
        {
            var cmpScan = GetCmpScan1();
            cmpScan.CopyRawDataToProcessed();
            var processing = new StraightenSynchronizationLine2();

            processing.Process(cmpScan);

            Assert.Equal(5, cmpScan.Data[0][1]);
            Assert.Equal(4, cmpScan.Data[1][1]);
            Assert.Equal(2, cmpScan.Data[2][1]);
        }
    }
}