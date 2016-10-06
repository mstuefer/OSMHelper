using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmHelper;

namespace OsmHelperTests
{
    [TestClass]
    public class MapHandlerTests
    {
        private MapHandler _mapHandler;
        private string _mapfileFirstLine;

        [TestInitialize]
        public void Initialize()
        {
            _mapHandler = new MapHandler("EuROpe","AndORRa");
            _mapfileFirstLine = "<?xml version='1.0' encoding='UTF-8'?>";
        }

        [TestMethod]
        public void GetOsmMapTest()
        {
            var stopwatch = new Stopwatch();

            _mapHandler.DeleteCachedMaps();

            stopwatch.Start();
            var mapFileStream = _mapHandler.GetOsmMap();
            var initialMapDownloadTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            Assert.IsNotNull(mapFileStream, "Got null instead of mapFileStream");
            Assert.IsTrue(mapFileStream.CanRead, "mapFileStream is not readable");

            var buffer = new byte[_mapfileFirstLine.Length];
            mapFileStream.Read(buffer, 0, _mapfileFirstLine.Length);

            var bufferString = System.Text.Encoding.Default.GetString(buffer); 

            Assert.IsTrue(bufferString.Equals(_mapfileFirstLine));

            stopwatch.Restart();
            _mapHandler.GetOsmMap();
            stopwatch.Stop();
            Trace.WriteLine($"init: {initialMapDownloadTime}, now: {stopwatch.ElapsedMilliseconds}");
            Assert.IsTrue(initialMapDownloadTime > stopwatch.ElapsedMilliseconds, "Initial Download was faster than getting file from cache (filesystem)");
        }
    }
}
