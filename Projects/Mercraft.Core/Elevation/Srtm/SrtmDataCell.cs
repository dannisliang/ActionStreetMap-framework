using System;
using System.IO;

namespace Mercraft.Core.Elevation.Srtm
{
    /// <summary>
    ///     Represents SRTM data cell.
    /// </summary>
    public class SrtmDataCell
    {
        private readonly byte[] _hgtData;
        private readonly int _pointsPerCell;

        private int _limit;

        /// <summary>
        ///     Gets or sets the latitude of the srtm data file.
        /// </summary>
        public int Latitude { get; private set; }

        /// <summary>
        ///     Gets or sets the longitude of the srtm data file.
        /// </summary>
        public int Longitude { get; private set; }

        public SrtmDataCell(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("File not found.", filepath);

            if (string.Compare(".hgt", Path.GetExtension(filepath), StringComparison.CurrentCultureIgnoreCase) != 0)
                throw new ArgumentException("Invalid extension.", filepath);

            string filename = Path.GetFileNameWithoutExtension(filepath).ToLower();
            string[] fileCoordinate = filename.Split(new[] {'e', 'w'});
            if (fileCoordinate.Length != 2)
                throw new ArgumentException("Invalid filename.", filepath);

            fileCoordinate[0] = fileCoordinate[0].TrimStart(new[] {'n', 's'});

            Latitude = int.Parse(fileCoordinate[0]);
            if (filename.Contains("s"))
                Latitude *= -1;

            Longitude = int.Parse(fileCoordinate[1]);
            if (filename.Contains("w"))
                Longitude *= -1;

            _hgtData = File.ReadAllBytes(filepath);

            switch (_hgtData.Length)
            {
                case 1201*1201*2: // SRTM-3
                    _pointsPerCell = 1201;
                    break;
                case 3601*3601*2: // SRTM-1
                    _pointsPerCell = 3601;
                    break;
                default:
                    throw new ArgumentException("Invalid file size.", filepath);
            }

            _limit = _pointsPerCell*_pointsPerCell*2;
        }

        public float GetElevation(double latitude, double longitude)
        {
            int localLat = (int) ((latitude - Latitude)*_pointsPerCell);
            int localLon = (int) (((longitude - Longitude))*_pointsPerCell);
            int bytesPos = ((_pointsPerCell - localLat - 1)*_pointsPerCell*2) + localLon*2;

            if (bytesPos < 0 || bytesPos > _limit)
                throw new ArgumentOutOfRangeException("latitude", "latitude or longitude is out of range.");

            // Motorola "big-endian" order with the most significant byte first
            return (_hgtData[bytesPos]) << 8 | _hgtData[bytesPos + 1];
        }
    }
}