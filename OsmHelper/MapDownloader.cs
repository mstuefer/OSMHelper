using System.Net;

namespace OsmHelper
{
    /// <summary>
    /// Downloads an OsmMap as an .osm (xml) from geofabrik.de
    /// </summary>
    public static class MapDownloader
    {
        private const string OsmExtractDownloadUrl = "http://download.geofabrik.de/{0}/{1}-latest.osm.bz2";

        /// <summary>
        /// Downloads the desired compressed (bz2) map from geofabrik.de (osm extract)
        /// and saves it to the desired destinationFile as an osmXml
        /// </summary>
        /// <param name="continent">Continent to download (e.g. Europe)</param>
        /// <param name="country">Country to download (e.g. Italy)</param>
        /// <param name="destinationFile">(e.g. Europe-Italy-latest.osm.bz2)</param>
        public static void Download(string continent, string country, string destinationFile)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(string.Format(OsmExtractDownloadUrl, continent, country).ToLower(), destinationFile);
        }
    }
}
