using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmHelper;
using OsmHelperInterfaces;

namespace OsmHelperTests
{
    [TestClass]
    public class InMemoryGraphTests
    {
        private IGraph _graph;
        private StringBuilder _osmXml;

        [TestInitialize]
        public void Initialize()
        {
            _graph = new InMemoryGraph();

            _osmXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            _osmXml.Append("<osm version=\"0.6\" generator=\"CGImap 0.4.3(7414 thorn - 03.openstreetmap.org)\" copyright=\"OpenStreetMap and contributors\" attribution=\"http://www.openstreetmap.org/copyright\" license=\"http://opendatacommons.org/licenses/odbl/1-0/\">");
            _osmXml.Append("<bounds minlat=\"46.4668700\" minlon=\"11.3262900\" maxlat=\"46.4717200\" maxlon=\"11.3381500\" />");
            _osmXml.Append("<node id=\"2268440800\" visible=\"true\" version=\"1\" changeset=\"15739427\" timestamp=\"2013-04-15T17:02:59Z\" user=\"pikappa79\" uid=\"330007\" lat=\"46.4677853\" lon=\"11.3361243\"/>");

            _osmXml.Append("<node id=\"2179592657\" visible=\"true\" version=\"2\" changeset=\"15939838\" timestamp=\"2013-05-01T21:58:42Z\" user=\"Skombi\" uid=\"1380953\" lat=\"46.4716178\" lon=\"11.3248174\" />");
            _osmXml.Append("<node id=\"292407691\" visible=\"true\" version=\"2\" changeset=\"38902841\" timestamp=\"2016 - 04 - 26T20: 47:58Z\" user=\"luschi\" uid=\"1714220\" lat=\"46.4711124\" lon=\"11.3265284\"/>");

            // First regular way to add
            _osmXml.Append("<way id=\"26659552\" visible=\"true\" version=\"9\" changeset=\"38902841\" timestamp=\"2016-04-26T20:47:59Z\" user=\"luschi\" uid=\"1714220\">");
            _osmXml.Append("<nd ref=\"2179592657\" />");
            _osmXml.Append("<nd ref= \"4149958705\" />");
            _osmXml.Append("<nd ref=\"292407691\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"unclassified\" />");
            _osmXml.Append("<tag k=\"name\" v=\"Via Antonio Stradivari - Antonio Stradivari Straße\" />");
            _osmXml.Append("<tag k=\"name:de\" v=\"Antonio Stradivari Straße\" />");
            _osmXml.Append("<tag k=\"name:it\" v=\"Via Antonio Stradivari\" />");
            _osmXml.Append("</way>");

            // First 'data error' way as before, has to be skipped
            _osmXml.Append("<way id=\"26659552\" visible=\"true\" version=\"9\" changeset=\"38902841\" timestamp=\"2016-04-26T20:47:59Z\" user=\"luschi\" uid=\"1714220\">");
            _osmXml.Append("<nd ref=\"2179592657\" />");
            _osmXml.Append("<nd ref= \"4149958705\" />");
            _osmXml.Append("<nd ref=\"292407691\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"unclassified\" />");
            _osmXml.Append("<tag k=\"name\" v=\"Via Antonio Stradivari - Antonio Stradivari Straße\" />");
            _osmXml.Append("<tag k=\"name:de\" v=\"Antonio Stradivari Straße\" />");
            _osmXml.Append("<tag k=\"name:it\" v=\"Via Antonio Stradivari\" />");
            _osmXml.Append("</way>");

            // Second regular way to add
            _osmXml.Append("<way id=\"26659562\" visible=\"true\" version=\"12\" changeset=\"32788646\" timestamp=\"2015-07-21T21:11:42Z\" user=\"Davlak\" uid=\"217070\" >");
            _osmXml.Append("<nd ref=\"292407611\" />");
            _osmXml.Append("<nd ref= \"2179592225\" />");
            _osmXml.Append("<nd ref= \"292407647\" />");
            _osmXml.Append("<nd ref= \"2475961366\" />");
            _osmXml.Append("<nd ref= \"292407648\" />");
            _osmXml.Append("<nd ref= \"292407649\" />");
            _osmXml.Append("<nd ref= \"292407650\" />");
            _osmXml.Append("<nd ref= \"2179591856\" />");
            _osmXml.Append("<nd ref= \"2475961365\" />");
            _osmXml.Append("<nd ref= \"292407652\" />");
            _osmXml.Append("<nd ref= \"2475961364\" />");
            _osmXml.Append("<nd ref= \"292407657\" />");
            _osmXml.Append("<nd ref= \"292407665\" />");
            _osmXml.Append("<nd ref= \"2475961363\" />");
            _osmXml.Append("<nd ref= \"2475961362\" />");
            _osmXml.Append("<nd ref= \"2475961361\" />");
            _osmXml.Append("<nd ref= \"2475961360\" />");
            _osmXml.Append("<nd ref= \"2475961359\" />");
            _osmXml.Append("<nd ref= \"2475961358\" />");
            _osmXml.Append("<nd ref= \"2475961357\" />");
            _osmXml.Append("<nd ref= \"292407672\" />");
            _osmXml.Append("<nd ref= \"292407669\" />");
            _osmXml.Append("<nd ref= \"2475961356\" />");
            _osmXml.Append("<nd ref= \"292407670\" />");
            _osmXml.Append("<nd ref= \"292407671\" />");
            _osmXml.Append("<tag k = \"bicycle\" v = \"no\" />");
            _osmXml.Append("<tag k = \"highway\" v = \"primary\" />");
            _osmXml.Append("<tag k = \"lanes\" v = \"2\" />");
            _osmXml.Append("<tag k = \"layer\" v = \"-1\" />");
            _osmXml.Append("<tag k = \"name\" v = \"Strada Statale dell'Abetone e del Brennero - Brenner-Staatsstraße\" />");
            _osmXml.Append("<tag k = \"name:de\" v = \"Brenner-Staatsstraße\" />");
            _osmXml.Append("<tag k = \"name:it\" v = \"Strada Statale dell'Abetone e del Brennero\" />");
            _osmXml.Append("<tag k = \"nat_ref\" v = \"SS12\" />");
            _osmXml.Append("<tag k = \"ref\" v = \"SS12\" />");
            _osmXml.Append("<tag k = \"tunnel\" v = \"yes\" />");
            _osmXml.Append("</way>");

            // Footway which has not to be considered
            _osmXml.Append("<way id=\"325313664\" visible=\"true\" version=\"1\" changeset=\"28494306\" timestamp=\"2015-01-29T21:13:16Z\" user=\"Tomi\" uid=\"30255\" >");
            _osmXml.Append("<nd ref=\"2279538555\" />");
            _osmXml.Append("<nd ref=\"3319844562\" />");
            _osmXml.Append("<nd ref=\"3319844561\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"footway\" />");
            _osmXml.Append("</way>");

            // Way 1 crossing Way 2
            _osmXml.Append("<way id=\"000000001\" visible=\"true\" version=\"1\" changeset=\"28494306\" timestamp=\"2015-01-29T21:13:16Z\" user=\"Tomi\" uid=\"30255\" >");
            _osmXml.Append("<nd ref=\"101\" />");
            _osmXml.Append("<nd ref=\"111\" />");
            _osmXml.Append("<nd ref=\"112\" />");
            _osmXml.Append("<nd ref=\"102\" />");
            _osmXml.Append("<nd ref=\"121\" />");
            _osmXml.Append("<nd ref=\"122\" />");
            _osmXml.Append("<nd ref=\"123\" />");
            _osmXml.Append("<nd ref=\"103\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"primary\" />");
            _osmXml.Append("</way>");

            // Way 2 crossing Way 1
            _osmXml.Append("<way id=\"000000002\" visible=\"true\" version=\"1\" changeset=\"28494306\" timestamp=\"2015-01-29T21:13:16Z\" user=\"Tomi\" uid=\"30255\" >");
            _osmXml.Append("<nd ref=\"104\" />");
            _osmXml.Append("<nd ref=\"102\" />");
            _osmXml.Append("<nd ref=\"106\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"secondary\" />");
            _osmXml.Append("</way>");

            // Add node which was used in way (id=26659552) only now
            _osmXml.Append(" <node id=\"4149958705\" visible=\"true\" version=\"1\" changeset=\"38902841\" timestamp=\"2016 - 04 - 26T20: 47:58Z\" user=\"luschi\" uid=\"1714220\" lat=\"46.4711607\" lon=\"11.3264479\" />");

            // OneWay
            _osmXml.Append("<way id=\"33272603\" visible=\"true\" version=\"6\" changeset=\"12522084\" timestamp=\"2012-07-28T11:41:25Z\" user=\"tyr_asd\" uid=\"115612\" >");
            _osmXml.Append("<nd ref= \"377898229\" />");
            _osmXml.Append("<nd ref= \"1794645028\" />");
            _osmXml.Append("<nd ref= \"1794645029\" />");
            _osmXml.Append("<nd ref= \"377898591\" />");
            _osmXml.Append("<tag k=\"highway\" v=\"primary\" />");
            _osmXml.Append("<tag k=\"lanes\" v=\"1\" />");
            _osmXml.Append("<tag k=\"maxspeed\" v=\"50\" />");
            _osmXml.Append("<tag k=\"name\" v=\"Via Einstein - Einsteinstraße\" />");
            _osmXml.Append("<tag k=\"name:de\" v=\"Einsteinstraße\" />");
            _osmXml.Append("<tag k=\"name:it\" v=\"Via Einstein\" />");
            _osmXml.Append("<tag k=\"oneway\" v=\"yes\" />");
            _osmXml.Append("</way>");

            _osmXml.Append("</osm>");
        }

        [TestCleanup]
        public void CleanUp()
        {
            _graph = null;
            _osmXml = null;
        }

        [TestMethod]
        [Timeout(100)]
        public void GraphTest()
        {
            // create new graph with 4 nodes and 2 edges (0->1, 1->0)
            _graph.AddNodeToGraph(0, 0, 0).AddNodeToGraph(1, 0, 0).AddNodeToGraph(2, 0, 0).AddNodeToGraph(3, 0, 0);
            _graph.AddEdge(0, 1, 10, false);

            Assert.AreEqual(4, _graph.CountNodes());
            Assert.AreEqual(2, _graph.CountEdges());

            // add 4 more edges (0<->2, 1<->3)
            _graph.AddEdge(0, 2, 10, false).AddEdge(1, 3, 10, false).AddEdge(3, 1, 10, false);

            Assert.AreEqual(4, _graph.CountNodes());
            Assert.AreEqual(6, _graph.CountEdges());

            // try to add already existing nodes
            _graph.AddNodeToGraph(0, 0, 0).AddNodeToGraph(3, 0, 0);

            Assert.AreEqual(4, _graph.CountNodes());
            Assert.AreEqual(6, _graph.CountEdges());
        }

        [TestMethod]
        [Timeout(200)]
        public void GetFromStreamTest()
        {
            _graph.GetFromXmlStream(new MemoryStream(Encoding.UTF8.GetBytes(_osmXml.ToString())));

            Assert.AreEqual(10, _graph.CountNodes());
            Assert.AreEqual(13, _graph.CountEdges());
            Assert.IsTrue(_graph.OsmGraph[2179592657].First.Value.Distance > 0.1430986);
        }

        [TestMethod]
        public void GraphDownloadTest()
        {
            _graph.Get("Europe", "Andorra", false);
            Assert.IsTrue(_graph.OsmGraph.Count > 7508, "There are less edges than ways in the raw data");

            //_graph = new Graph();
            //_graph.Get("Europe", "Austria");
            //_graph.GetFromOsmApi(11.32629f, 46.46687f, 11.33815f, 46.47172f); // bolzano sued, very small part
            //_graph.GetFromOsmApi(5.422f, 42.403f, 16.188f, 47.395f); // north italy
            // 42.403f, 5.422, 47.395, 16.188
        }
    }
}
