using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmHelper;

namespace OsmHelperTests
{
    [TestClass]
    public class MapDownloaderTests
    {
        [TestMethod]
        public void DownloadTest()
        {
            const string continent = "EuRope";
            const string country = "AndoRRA";
            const string filename = "EuRope-AndoRRA-latest.osm.bz2";

            MapDownloader.Download(continent, country, filename);

            Assert.IsTrue(File.Exists(filename), $"File {filename} was not Downloaded");
        }
    }
}
