using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmpCurvesSummation.Core;

namespace GprFileService
{
    /// <summary>
    /// To open all types of files
    /// </summary>
    public interface IFileOpener
    {
        ICmpScan OpenKrotTxt(string filepath);
        ICmpScan OpenGeo1(string filepath);
        ICmpScan OpenGem(string filepath);
    }

    

    public class FileOpener : IFileOpener
    {
        public ICmpScan OpenKrotTxt(string filepath) => new TxtKrotFileOpener().OpenFile(filepath);

        public ICmpScan OpenGeo1(string filepath)
        {
            var geoFileType = new GeoFileAnalyzer().GetGeoFileType(filepath);
            switch (geoFileType)
            {
                case GeoFileType.MainHeader16AscanHeader10After1Length256:
                    return new GeoFileOpener().OpenFile(filepath, 10, 256);
                case GeoFileType.MainHeader16AscanHeader10After1Length512:
                    return new GeoFileOpener().OpenFile(filepath, 10, 512);
                case GeoFileType.MainHeader16AscanHeader13After1Length256:
                    return new GeoFileOpener().OpenFile(filepath, 13, 256);
                case GeoFileType.MainHeader16AscanHeader13After1Length512:
                    return new GeoFileOpener().OpenFile(filepath, 13, 512);
                default:
                    throw new FileFormatException("Wrong file format");
            }
        }
        
        public ICmpScan OpenGem(string filepath) => new GemFileOpener().OpenFile(filepath);
    }


    public enum GeoFileType
    {
        MainHeader16AscanHeader10After1Length256,
        MainHeader16AscanHeader10After1Length512,
        MainHeader16AscanHeader13After1Length256,
        MainHeader16AscanHeader13After1Length512
    }


    public class GeoFileAnalyzer
    {
        public GeoFileType GetGeoFileType(string filepath)
        {
            byte[] data;
            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
            }

            var ascanHeaderPart = new byte[] { 0xA1, 0x00 };

            if (data.Length < 2)
                throw new ArgumentOutOfRangeException("impossibly short file");

            var indices = new List<int>();
            for (int i = 0; i < data.Length - 2; i++)
                if (data[i] == ascanHeaderPart[0] && data[i + 1] == ascanHeaderPart[1])
                    indices.Add(i);

            var gaps = new List<int>();
            for (int i = 2; i < indices.Count; i++)
                gaps.Add(indices[i] - indices[i - 1]);
            var gap = gaps.Max();

            if (indices[0] == 16 && gap == 267)
                return GeoFileType.MainHeader16AscanHeader10After1Length256;
            else if (indices[0] == 16 && gap == 523)
                return GeoFileType.MainHeader16AscanHeader10After1Length512;
            else if (indices[0] == 16 && gap == 270)
                return GeoFileType.MainHeader16AscanHeader13After1Length256;
            else if (indices[0] == 16 && gap == 526)
                return GeoFileType.MainHeader16AscanHeader13After1Length512;
            else throw new FileFormatException("something's wrong with file format");
        }
    }
}