using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmpFileService;
using Xunit;

namespace CmpCurvesSummation.Core.Tests
{
    public class FileOpenerTests
    {
        private const string filename = "qwertyuiop098765.txt";


        private void CreateBasicTxtFile()
        {
            var data = new List<string>();

            data.Add("X;T;ALg1;");
            data.Add("0;0;2511;");
            data.Add("0;0,5;2519;");
            data.Add("0;1;2513;");
            data.Add("1;0;2519;");
            data.Add("1;0,5;2517;");
            data.Add("1;1;2519;");
            data.Add("2;0;0;");
            data.Add("2;0,5;0;");
            data.Add("2;1;0;");
            File.WriteAllLines(filename, data);
        }

        private void CreateBadNotEnoughNumbersTxtFile()
        {
            var data = new List<string>();

            data.Add("X;T;ALg1;");
            data.Add("0;0;2511;");
            data.Add("0;0,52519;");
            data.Add("0;1;2513;");
            data.Add("1;0;2519;");
            data.Add("1;0,5;2517;");
            data.Add("1;1;2519;");
            File.WriteAllLines(filename, data);
        }


        [Fact]
        public void OpenTxtSimple()
        {
            CreateBasicTxtFile();
            var opener = new FileOpener();

            var cmpScan = opener.OpenKrotTxt(filename);

            Assert.Equal(2, cmpScan.LengthDimensionless);
            Assert.Equal(3, cmpScan.AscanLengthDimensionless);
        }

        [Fact]
        public void OpenBadNotEnoughNumbersTxtFile()
        {
            CreateBadNotEnoughNumbersTxtFile();
            var opener = new FileOpener();

            Assert.Throws<FileLoadException>(() => opener.OpenKrotTxt(filename));
        }
    }
}
