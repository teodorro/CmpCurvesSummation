using Xunit;

namespace CmpCurvesSummation.Core.Tests
{
    public class SummedScanTests
    {
        [Fact]
        public void Summation()
        {
            var cmpScan = GetSimpleCmpScan();

            var summedScan = new SummedScanVH(cmpScan);
            var data = summedScan.Data;

            // assert smth
        }

        private ICmpScan GetSimpleCmpScan()
        {
            var cmpScan = new CmpScan();
            var cmpLength = 5;
            var ascanLength = 5;

            for (int i = 0; i < cmpLength; i++)
            {
                for (int j = 0; j < ascanLength; j++)
                {

                }
            }

            return cmpScan;
        }
    }
}