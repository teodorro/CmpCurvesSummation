using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpCurvesSummation.Core;
using Xunit;

namespace CmpCurvesSummation.Core.Tests
{
    public class CmpMathTests
    {
//        [Fact]
//        public void TestHodographLineClassic()
//        {
//            var h = 4.0;
//            var d = 3 * 2.0;
//            var v = 5.0;
//
//            var t = CmpMath.Instance.HodographLineClassic(d, h, v);
//
//            Assert.Equal(0.5, t);
//        }

        [Fact]
        public void TestHodographLineLoza()
        {
            var h = 2.0;
            var d = 3.0;
            var v = 5.0;

            var t = CmpMath.Instance.HodographLineLoza(d, h, v);

            Assert.Equal(-9.5, t);
        }

        [Fact]
        public void TestLayerVelocity()
        {
            var v2 = CmpMath.Instance.LayerVelocity(13.4, 3, 4, 0.1333, 2, 0.1);

            Assert.Equal(0.2, Math.Round(v2, 2));
        }
    }
}
