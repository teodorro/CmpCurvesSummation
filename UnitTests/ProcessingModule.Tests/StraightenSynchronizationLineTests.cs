﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using ProcessingModule.Processing;
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

        [Fact]
        public void AscansMove()
        {
            var cmpScan = GetSimpleCmpScan();
            cmpScan.CopyRawDataToProcessed();
            var processing = new StraightenSynchronizationLine();

            processing.Process(cmpScan);

            Assert.Equal(1, cmpScan.Data[0][4]);
            Assert.Equal(1, cmpScan.Data[1][4]);
            Assert.Equal(1, cmpScan.Data[2][4]);
        }
    }
}
