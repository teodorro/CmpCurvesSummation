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
        [Fact]
        public void TestHodographLineClassic()
        {
            var h = 4.0;
            var d = 3 * 2.0;
            var v = 5.0;

            var t = CmpMath.Instance.HodographLineClassic(d, h, v);

            Assert.Equal(1.0, t);
        }

        [Fact]
        public void TestHodographLineLoza()
        {
            var h = 4.0;
            var d = 3 * 2.0;
            var v = 5.0;

            var t = CmpMath.Instance.HodographLineLoza(d, h, v);

            Assert.Equal(-19.0, t);
        }

        [Fact]
        public void TestLayerVelocity()
        {

        }
    }
}
