using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using OsmHelperInterfaces;

namespace OsmHelper
{
    /// <summary>
    /// Returns a desired map for a given continent and country.
    /// </summary>
    public class MapHandler : IMapHandler
    {
        private readonly string _continent;
        private readonly string _country;
        private readonly string _compressedFilename;
        private readonly string _filename;

        public MapHandler(string continent, string country)
        {
            _continent = continent;
            _country = country;
            _compressedFilename = $"{continent}-{country}-latest.osm.bz2";
            _filename = _compressedFilename.Substring(0, _compressedFilename.Length - 4);
        }

        /// <summary>
        /// Deletes cached maps (from filesystem)
        /// </summary>
        public void DeleteCachedMaps()
        {
            if(File.Exists(_filename))
                File.Delete(_filename);
            if(File.Exists(_compressedFilename))
                File.Delete(_compressedFilename);
        }

        /// <summary>
        /// Returns Filestream to desired OsmMap (xml)
        /// </summary>
        /// <returns></returns>
        public FileStream GetOsmMap()
        {
            if (GetCachedOsmMap() == null)
                RefreshMap();

            return GetCachedOsmMap();
        }

        private FileStream GetCachedOsmMap()
        {
            if (!File.Exists(_filename) && File.Exists(_compressedFilename))
                Bunzip(_compressedFilename);

            return File.Exists(_filename) ? File.OpenRead(_filename) : null;
        }

        private static void Bunzip(string destinationFile)
        {
            var decompressedDestinationFile = destinationFile.Substring(0, destinationFile.Length - 4);

            using (var compressedFileStream = File.OpenRead(destinationFile))
            {
                using (var decompressedFileStream = File.Create(decompressedDestinationFile))
                {
                    BZip2.Decompress(compressedFileStream, decompressedFileStream, true);
                }
            }
        }

        /// <summary>
        /// Downloads new map, overriding old (cached) ones.
        /// </summary>
        public void RefreshMap()
        {
            MapDownloader.Download(_continent, _country, _compressedFilename);
        }
    }
}
