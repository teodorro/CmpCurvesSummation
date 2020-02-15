using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
using ProcessingModule.Processing.CmpScan;
using Xunit;

namespace ProcessingModule.Tests
{
    public class StraightenSynchronizationLineTests
    {
        public ICmpScan GetSimpleCmpScan()
        {
            var a = 3;
            var cmpScan = new CmpScan();
            for (int i = 0; i < a; i++)
            {
                var ascan = new double[2*a];
                cmpScan.RawData.Add(ascan);
                ascan[i + 2] = 1;
            }

            return cmpScan;
        }

        public ICmpScan GetSimpleCmpScanMinus()
        {
            var a = 3;
            var cmpScan = new CmpScan();
            for (int i = 0; i < a; i++)
            {
                var ascan = new double[2 * a];
                cmpScan.RawData.Add(ascan);
                ascan[i + 2] = -1;
            }

            return cmpScan;
        }

        public ICmpScan GetSimpleCmpScanFlat()
        {
            var cmpScan = new CmpScan();
            var ascan1 = new double[] { 0, 1, 1, 1, 0 };
            var ascan2 = new double[] { 0, 0, 0, 1, 0 };
            var ascan3 = new double[] { 0, 1, 0, 0, 0 };
            cmpScan.RawData.Add(ascan1);
            cmpScan.RawData.Add(ascan2);
            cmpScan.RawData.Add(ascan3);

            return cmpScan;
        }

        [Fact]
        public void AscansMove()
        {
            var cmpScan = GetSimpleCmpScan();
            cmpScan.CopyRawDataToProcessed();
            var processing = new StraightenSynchronizationLine(0);

            processing.Process(cmpScan);

            Assert.Equal(1, cmpScan.Data[0][4]);
            Assert.Equal(1, cmpScan.Data[1][4]);
            Assert.Equal(1, cmpScan.Data[2][4]);
        }

        [Fact]
        public void AscansMoveMinus()
        {
            var cmpScan = GetSimpleCmpScanMinus();
            cmpScan.CopyRawDataToProcessed();
            var processing = new StraightenSynchronizationLine(0);

            processing.Process(cmpScan);

            Assert.Equal(-1, cmpScan.Data[0][4]);
            Assert.Equal(-1, cmpScan.Data[1][4]);
            Assert.Equal(-1, cmpScan.Data[2][4]);
        }

        [Fact]
        public void AscansMoveFlat()
        {
            var cmpScan = GetSimpleCmpScanFlat();
            cmpScan.CopyRawDataToProcessed();
            var processing = new StraightenSynchronizationLine(0);

            processing.Process(cmpScan);
            
            Assert.Equal(1, cmpScan.Data[0][3]);
            Assert.Equal(1, cmpScan.Data[1][3]);
            Assert.Equal(1, cmpScan.Data[2][3]);
        }
    }
}
