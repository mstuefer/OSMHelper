using System.Collections.Generic;
using System.IO;
using System.Xml;
using OsmHelperInterfaces;

namespace OsmHelper
{
    /// <summary>
    /// Represents the desired map as a Graph (Adjacency List)
    /// </summary>
    public abstract class Graph : IGraph
    {
        private XmlTextReader _osmMap;

        private readonly HashSet<string> _prohibitedWaysForMotorizedVehicles;
        private MapHandler _mapHandler;

        /// <summary>
        /// Final Graph (Adjacency List)
        /// </summary>
        public IDictionary<long, LinkedList<INode>> OsmGraph { get; protected set; }

        protected Graph()
        {
            OsmGraph = new Dictionary<long, LinkedList<INode>>();
            _prohibitedWaysForMotorizedVehicles = ProhibitedWays();
        }

        /// <summary>
        /// Returns a StringHashSet of all Ways which are prohibited to traverse with a car.
        /// </summary>
        /// <returns></returns>
        private static HashSet<string> ProhibitedWays()
        {
            // http://wiki.openstreetmap.org/wiki/Map_Features
            return new HashSet<string>
            {
                "pedestrian",
                "track",
                "bus_guide",
                "raceway",
                "footway",
                "bridleway",
                "steps",
                "path",
                "sidewalk",
                "cycleway",
                "proposed",
                "construction",
                "bus_stop",
                "crossing",
                "elevator",
                "emergency_access_point",
                "escape",
                "give_way",
                "mini_roundabout",
                "passing_place",
                "rest_area",
                "speed_camera",
                "street_lamp",
                "services",
                "stop",
                "traffic_signals",
                "turning_circle"
            };
        }

        /// <summary>
        /// Generates the graph from the given stream
        /// </summary>
        /// <param name="osmXml">An XmlStream in the open street map form</param>
        public void GetFromXmlStream(Stream osmXml)
        {
            _osmMap = new XmlTextReader(osmXml);
            GenerateGraph();
        }

        /// <summary>
        /// Invokes everything necessary to get the desired Graph (Adj List) from the given continent and country.
        /// Uses possibly cached maps if deleteCachedMaps set to false (default).
        /// </summary>
        /// <param name="continent"></param>
        /// <param name="country"></param>
        /// <param name="deleteCachedMaps"></param>
        public void Get(string continent, string country, bool deleteCachedMaps = false)
        {
            _mapHandler = new MapHandler(continent, country);
            if (deleteCachedMaps) _mapHandler.DeleteCachedMaps();

            var mapFileStream = _mapHandler.GetOsmMap();
            _osmMap = new XmlTextReader(mapFileStream);

            GenerateGraph();
            mapFileStream.Close();
        }

        private void GenerateGraph()
        {
            ParseMap();
            AddEdges();
            CutNodesWithoutNeighbors();
            OrderGraph();
        }

        /*
        private void OrderGraph()
        {
            var orderedGraph = new Dictionary<long, LinkedList<INode>>();
            foreach (var node in OsmGraph)
                orderedGraph[node.Key] = new LinkedList<INode>(node.Value.OrderBy(n => n.Distance));

            OsmGraph = orderedGraph;
        }
        */

        protected abstract void OrderGraph();

        private void ParseMap()
        {
            while (_osmMap.Read())
            {
                if (_osmMap.Name.Equals("way"))
                    AddOsmWay(_osmMap);
                else if (_osmMap.Name.Equals("node"))
                    AddOsmNode(_osmMap);
            }
        }

        /*
        private void AddEdges()
        {
            foreach (var way in _ways)
                AddWayToEdges(way.Value);
        }
        */

        protected abstract void AddEdges();

        protected abstract void AddWayToEdges(IWay way);

        /*
        private void AddOsmWay(XmlReader reader)
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
        */

        protected abstract void AddOsmWay(XmlReader reader);

        private void AddOsmNode(XmlReader reader)
        {
            var visible = reader.GetAttribute("visible");
            if (IsNotVisible(visible)) return;

            var id = reader.GetAttribute("id");
            var lat = reader.GetAttribute("lat");
            var lon = reader.GetAttribute("lon");
            if (id != null && lat != null && lon != null)
                AddNode(long.Parse(id), float.Parse(lat), float.Parse(lon));
        }

        protected static bool IsNotVisible(string visible)
        {
            return !IsVisible(visible);
        }

        private static bool IsVisible(string visible)
        {
            return string.IsNullOrEmpty(visible) || visible.Equals("true");
        }

        protected bool IsNotForMotorizedVehicles(string highway)
        {
            return !IsForMotorizedVehicles(highway);
        }

        private bool IsForMotorizedVehicles(string highway)
        {
            // Assumption empty string is for motorized vehicles (as written in OSM Wiki)
            return string.IsNullOrEmpty(highway) || !_prohibitedWaysForMotorizedVehicles.Contains(highway);
        }

        /*
        private void CutNodesWithoutNeighbors()
        {
            var nodesToRemove = OsmGraph.AsParallel().Where(node => node.Value.Count == 0).ToList();
            foreach (var node in nodesToRemove)
                OsmGraph.Remove(node.Key);
        }
        */

        protected abstract void CutNodesWithoutNeighbors();

        /*
        private INode AddNode(long id, bool increaseCounter = false)
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
        */

        protected abstract INode AddNode(long id, bool increaseCounter);

        /*
        private void AddNode(long id, float lat, float lon)
        {
            var node = AddNode(id);
            node.Latitude = lat;
            node.Longitude = lon;
        }
        */

        protected abstract void AddNode(long id, float lat, float lon);

        /*
        public IGraph AddNodeToGraph(long id, float lat, float lon)
        {
            if (OsmGraph.ContainsKey(id)) return this;
            OsmGraph.Add(id, new LinkedList<INode>());
            _nodes.Add(id, new Node(id, lat, lon));
            return this;
        }
        */

        public abstract IGraph AddNodeToGraph(long id, float lat, float lon);

        /// <summary>
        /// Adds Edge from source node with id (srcNodeId) to destination node with id (dstNodeId) setting distance between them as given.
        /// If Edge is traversable only in one direction, set oneWay to true.
        /// </summary>
        /// <param name="srcNodeId">Source Node Id</param>
        /// <param name="dstNodeId">Destinatin Node Id</param>
        /// <param name="distance">Distance between srcNode and dstNode</param>
        /// <param name="oneWay">True if traversable in one direction, otherwise false (default)</param>
        /// <returns></returns>
        public IGraph AddEdge(long srcNodeId, long dstNodeId, double distance, bool oneWay = false)
        {
            AddDirectedEdge(srcNodeId, dstNodeId, distance);
            if (oneWay) return this;
            AddDirectedEdge(dstNodeId, srcNodeId, distance);

            return this;
        }

        /*
        private void AddDirectedEdge(long srcNodeId, long dstNodeId, double distance)
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
        */

        protected abstract void AddDirectedEdge(long srcNodeId, long dstNodeId, double distance);

        // public long CountNodes() => OsmGraph.Count;

        public abstract long CountNodes();

        // public long CountEdges() => OsmGraph.Sum(node => node.Value.Count);

        public abstract long CountEdges();
    }
}
