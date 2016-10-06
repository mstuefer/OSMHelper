using System.Collections.Generic;
using System.IO;

namespace OsmHelperInterfaces
{
    public interface IGraph
    {
        void GetFromXmlStream(Stream osmXml);
        IGraph AddNodeToGraph(long id, float lat, float lon);
        IGraph AddEdge(long srcNodeId, long dstNodeId, double distance, bool oneWay);
        long CountNodes();
        void Get(string continent, string country, bool deleteCachedMaps);
        long CountEdges();
        IDictionary<long,LinkedList<INode>> OsmGraph { get; }
    }
}
