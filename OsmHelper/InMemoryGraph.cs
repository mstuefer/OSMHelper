using System.Collections.Generic;
using System.Linq;
using System.Xml;
using OsmHelperInterfaces;

namespace OsmHelper
{
    /// <summary>
    /// Generates the graph InMemory.
    /// </summary>
    public sealed class InMemoryGraph : Graph
    {
        private readonly IDictionary<long, INode> _nodes; // The nodes in the given osmXml
        private readonly IDictionary<long, IWay> _ways; // The ways in the given osmXml

        public InMemoryGraph()
        {
            _nodes = new Dictionary<long, INode>();
            _ways = new Dictionary<long, IWay>();
        }

        /// <summary>
        /// Orders the graph s.t. all nodes are ordered according to their id, and
        /// all neighbors per node are ordered according to their distance to the node.
        /// </summary>
        protected override void OrderGraph()
        {
            var orderedGraph = new Dictionary<long, LinkedList<INode>>();
            foreach (var node in OsmGraph)
                orderedGraph[node.Key] = new LinkedList<INode>(node.Value.OrderBy(n => n.Distance));

            OsmGraph = orderedGraph;
        }

        /// <summary>
        /// Takes all ways we found in the osm xml and adds them as edges to the final graph.
        /// </summary>
        protected override void AddEdges()
        {
            foreach (var way in _ways)
                AddWayToEdges(way.Value);
        }

        protected override void AddWayToEdges(IWay way)
        {
            double distance = 0;
            long startId = -1, lastId = -1;
            var ndsCounter = 0;
            foreach (var edge in way.NdsList)
            {
                ndsCounter += 1;
                if (startId == -1)
                {
                    startId = lastId = edge;
                    continue;
                }
                if (!_nodes.ContainsKey(edge)) continue; // data error

                if (_nodes.ContainsKey(lastId) && _nodes.ContainsKey(edge))
                    distance += DistanceCalculator.GetDistance(_nodes[lastId], _nodes[edge]);

                // on intermediate edges go ahead (OR) on last edge, go ahead s.t. the edge will be added after the loop
                if (_nodes[edge].Counter <= 1 || ndsCounter >= way.NdsList.Count)
                {
                    lastId = edge;
                    continue;
                }

                // The node got used more than once, edge complete, open new edge
                AddEdge(startId, edge, distance, way.OneWay);
                startId = lastId = edge;
                distance = 0;
            }
            // Add last edge in way
            AddEdge(startId, lastId, distance, way.OneWay);
        }


        /// <summary>
        /// Adds a given way in the osm xml to our ways.
        /// </summary>
        /// <param name="reader"></param>
        protected override void AddOsmWay(XmlReader reader)
        {
            var wayId = reader.GetAttribute("id");
            var visible = reader.GetAttribute("visible");

            if (string.IsNullOrEmpty(wayId) || IsNotVisible(visible) || _ways.ContainsKey(long.Parse(wayId))) return;

            var way = new Way {Id = long.Parse(wayId)};

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("way"))
                    break; // way ends here

                if (reader.NodeType != XmlNodeType.Element) continue;

                if (reader.Name.Equals("nd"))
                {
                    var ndId = reader.GetAttribute("ref");
                    if (ndId == null) continue; // data error
                    way.NdsList.Add(long.Parse(ndId));
                }
                else if (reader.Name.Equals("tag"))
                {
                    var key = reader.GetAttribute("k");
                    if (key == null || !(key == "highway" || key == "oneway")) continue; // we are currently interested only in highways

                    if (key == "highway")
                    {
                        var value = reader.GetAttribute("v");
                        if (IsNotForMotorizedVehicles(value)) return;
                    }
                    else
                        way.OneWay = reader.GetAttribute("v") == "yes";
                }
            }
            // AddNode only now, s.t. we do not keep nodes which are not used since not for Motorized Vehicles
            foreach (var nd in way.NdsList)
                AddNode(nd, true);
            _ways.Add(way.Id, way);
        }

        /// <summary>
        /// Deletes all nodes without neighbors from our final graph.
        /// </summary>
        protected override void CutNodesWithoutNeighbors()
        {
            var nodesToRemove = OsmGraph.AsParallel().Where(node => node.Value.Count == 0).ToList();
            foreach (var node in nodesToRemove)
                OsmGraph.Remove(node.Key);
        }

        /// <summary>
        /// Adds node with given id if not already present in our nodes list.
        /// Increases counter on node to understand if node is an intersection of ways/edges. 
        /// (Intersection: all nodes with counter >= 1)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="increaseCounter"></param>
        /// <returns></returns>
        protected override INode AddNode(long id, bool increaseCounter = false)
        {
            if (_nodes.ContainsKey(id))
            {
                if (increaseCounter) _nodes[id].Counter += 1;
                return _nodes[id];
            }

            _nodes.Add(id, new Node(id));
            if (increaseCounter) _nodes[id].Counter += 1;
            return _nodes[id];
        }

        /// <summary>
        /// Adds node with given id if not already present in our nodes list.
        /// Adds/Updates latitude and longitude on node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        protected override void AddNode(long id, float lat, float lon)
        {
            var node = AddNode(id);
            node.Latitude = lat;
            node.Longitude = lon;
        }

        /// <summary>
        /// Adds Node to final graph (adj list).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <returns></returns>
        public override IGraph AddNodeToGraph(long id, float lat, float lon)
        {
            if (OsmGraph.ContainsKey(id)) return this;
            OsmGraph.Add(id, new LinkedList<INode>());
            _nodes.Add(id, new Node(id, lat, lon));
            return this;
        }

        /// <summary>
        /// Adds Edge from given source (srcNodeId) to destinatio (dstNodeId) with given distance.
        /// </summary>
        /// <param name="srcNodeId">Source Node Id</param>
        /// <param name="dstNodeId">Destination Node Id</param>
        /// <param name="distance">Distance between srcNode and dstNode</param>
        protected override void AddDirectedEdge(long srcNodeId, long dstNodeId, double distance)
        {
            var dstNode = _nodes.ContainsKey(dstNodeId) ? _nodes[dstNodeId] : new Node(dstNodeId);
            dstNode.Distance = distance;
            if (!OsmGraph.ContainsKey(srcNodeId))
            {
                OsmGraph.Add(srcNodeId, new LinkedList<INode>());
                OsmGraph[srcNodeId].AddFirst(dstNode);
            }
            else if (OsmGraph[srcNodeId].All(n => n.Id != dstNodeId)) // edge not present
                OsmGraph[srcNodeId].AddFirst(dstNode);
            // else, edge already present, therefore nothing to do anymore
        }

        /// <summary>
        /// Returns amount of nodes in graph.
        /// </summary>
        /// <returns></returns>
        public override long CountNodes() => OsmGraph.Count;

        /// <summary>
        /// Returns amount of edges in graph.
        /// </summary>
        /// <returns></returns>
        public override long CountEdges() => OsmGraph.Sum(node => node.Value.Count);
    }
}
