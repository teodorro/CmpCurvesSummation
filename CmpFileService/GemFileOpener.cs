using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmpCurvesSummation.Core;

namespace GprFileService
{
    public class GemFileOpener : IFileTypeOpener
    {
        public ICmpScan OpenFile(string filepath)
        {
            try
            {
                var cmpScan = ReadFileAfter2013(filepath);
                RemoveEmptyAscans(cmpScan);

                if (cmpScan.RawData.Count == 0)
                {
                    throw new FileLoadException("Ошибка при загрузке файла", filepath);
                    cmpScan = ReadFileBefore2013(filepath);
                    RemoveEmptyAscans(cmpScan);
                }

                return cmpScan;
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Ошибка при загрузке файла " + ex.Message, filepath);
            }
        }

        private CmpScan ReadFileBefore2013(string filepath)
        {
            var cmpScan = new CmpScan();
            var data = new List<byte[]>();
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                int length = 0;

                var krot = ReadKrot(fs);
                var unknown = ReadUnknown(fs);
                var bscanLength = ReanBscanLength(out length, fs);
                var unknown2 = ReadUknown2(fs);

                for (int i = 0; i < bscanLength; i++)
                {
                    var ascan = ReadAscan(out length, fs);
                    var ascanFooter = ReadScanFooterBefore2013(out length, fs);
                    data.Add(ascan);
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                cmpScan.RawData.Add(new double[data[i].Length]);
                for (int j = 0; j < data[i].Length; j++)
                    cmpScan.RawData[i][j] = data[i][j];
            }

            return cmpScan;
        }

        private CmpScan ReadFileAfter2013(string filepath)
        {
            var cmpScan = new CmpScan();
            var data = new List<byte[]>();
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                int length = 0;

                var krot = ReadKrot(fs);
                var unknown = ReadUnknown(fs);
                var bscanLength = ReanBscanLength(out length, fs);
                var unknown2 = ReadUknown2(fs);

                for (int i = 0; i < bscanLength; i++)
                {
                    var ascan = ReadAscan(out length, fs);
                    var ascanFooter = ReadScanFooterAfter2013(out length, fs);
                    data.Add(ascan);
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                cmpScan.RawData.Add(new double[data[i].Length]);
                for (int j = 0; j < data[i].Length; j++)
                    cmpScan.RawData[i][j] = data[i][j];
            }

            return cmpScan;
        }

        private byte[] ReadScanFooterAfter2013(out int length, FileStream fs)
        {
            length = 112;
            var info = new byte[length];
            fs.Read(info, 0, length);
            return info;
        }

        private byte[] ReadScanFooterBefore2013(out int length, FileStream fs)
        {
            length = 96;
            var info = new byte[length];
            fs.Read(info, 0, length);
            return info;
        }

        private byte[] ReadAscan(out int length, FileStream fs)
        {
            length = 512;
            var ascan = new byte[length];
            fs.Read(ascan, 0, length);
            return ascan;
        }

        private byte[] ReadUknown2(FileStream fs)
        {
            int length = 496;
            var unknown = new byte[length];
            fs.Read(unknown, 0, length);
            return unknown;
        }

        private int ReanBscanLength(out int length, FileStream fs)
        {
            length = 2;
            var bscanLengthArray = new byte[length];
            fs.Read(bscanLengthArray, 0, length);
            return bscanLengthArray[1] + bscanLengthArray[0] * 8;
        }

        private byte[] ReadUnknown(FileStream fs)
        {
            int length = 12;
            var unknown = new byte[length];
            fs.Read(unknown, 0, length);
            return unknown;
        }

        private byte[] ReadKrot(FileStream fs)
        {
            int length = 4;
            var krot = new byte[length];
            fs.Read(krot, 0, length);
            return krot;
        }

        /// <summary>
        /// Krot software create convert data to a list of 100 ascan packs. 
        /// If the length of isn't equal to *00, some empty ascans are appended ot make it to be equal.
        /// 0 is not minimum amplitude but it signals that it's an empty ascan
        /// </summary>
        private void RemoveEmptyAscans(ICmpScan cmpScan)
        {
            int emptyAscanIndex = 0;
            for (int i = 0; i < cmpScan.RawData.Count; i++)
                if (cmpScan.RawData[i].All(x => x == 0))
                {
                    emptyAscanIndex = i;
                    break;
                }
            cmpScan.RawData.RemoveRange(emptyAscanIndex, cmpScan.RawData.Count - emptyAscanIndex);
        }
    }
}