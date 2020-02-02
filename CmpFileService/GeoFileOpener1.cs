using System;
using System.Collections.Generic;
using System.IO;
using CmpCurvesSummation.Core;

namespace GprFileService
{
    public class GeoFileOpener1
    {
        public ICmpScan OpenFile(string filepath)
        {
            try
            {
                var cmpScan = new CmpScan();
                var data = new List<byte[]>();
                using (var fs = new FileStream(filepath, FileMode.Open))
                {
                    int length = 0;

                    var krot = ReadKrot(fs);
                    var unknown = ReadUnknown(fs);
                    var date = ReadDate(fs);
                    var unknown2 = ReadUknown2(fs);
                    var bscanLength = ReanBscanLength(out length, fs);
                    var unknown3 = new byte[1];

                    for (int i = 0; i < bscanLength; i++)
                    {
                        var ascanHeader = ReadScanHeader(out length, fs);
                        var ascan = ReadAscan(out length, fs);
                        data.Add(ascan);
                        fs.Read(unknown3, 0, 1);
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
            catch (Exception ex)
            {
                throw new FileLoadException("Ошибка при загрузке файла " + ex.Message, filepath);
            }
        }

        private byte[] ReadAscan(out int length, FileStream fs)
        {
            length = 255;
            var ascan = new byte[length];
            fs.Read(ascan, 0, length);
            return ascan;
        }

        private byte[] ReadScanHeader(out int length, FileStream fs)
        {
            length = 11;
            var ascanHeader = new byte[length];
            fs.Read(ascanHeader, 0, length);
            return ascanHeader;
        }

        private int ReanBscanLength(out int length, FileStream fs)
        {
            length = 3;
            var bscanLengthArray = new byte[length];
            fs.Read(bscanLengthArray, 0, length);
            return bscanLengthArray[2] + bscanLengthArray[1] * 8 + bscanLengthArray[0] * 64;
        }

        private byte[] ReadUknown2(FileStream fs)
        {
            int length;
            length = 3;
            var unknown2 = new byte[length];
            fs.Read(unknown2, 0, length);
            return unknown2;
        }

        private byte[] ReadDate(FileStream fs)
        {
            int length;
            length = 3;
            var date = new byte[length];
            fs.Read(date, 0, length);
            return date;
        }

        private byte[] ReadUnknown(FileStream fs)
        {
            int length = 3;
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
    }
}