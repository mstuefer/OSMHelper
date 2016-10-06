using Microsoft.Azure.Documents;
using OsmHelperInterfaces;
using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;

namespace OsmHelper
{
    /// <summary>
    /// Represents a Node in our OSM Graph
    /// </summary>
    public class Node : INode
    {

        public Node()
        {
            // empty constructor which we need to deserialize data from DocumentDb
        }

        /// <summary>
        /// Creates a Node with given id. 
        /// Distance has to be set if specific node instance is used to represent a neighbor in the adjacency list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="distance"></param>
        public Node(long id, double distance = -1)
        {
            Id = id;
            Distance = distance;
        }

        /// <summary>
        /// Creates a Node with given id, latitude and longitude.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public Node(long id, float latitude, float longitude) : this(id)
        {
            Latitude = latitude;
            Longitude = longitude;
            Location = new Point(longitude, latitude);
        }

        /// <summary>
        /// Creates a node with given id, latitude, longitude and distance.
        /// Distance represents the distance from a node to this one (neighbor) in the adjacency list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="distance"></param>
        public Node(long id, float latitude, float longitude, double distance)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            Distance = distance;
            Location = new Point(longitude, latitude);
        }

        [JsonConverter(typeof(LongStringConverter))]
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("counter")]
        /// <summary>
        /// Counter > 1 indicates node is intersection.
        /// </summary>
        public int Counter { get; set; }

        [JsonProperty("location")]
        private Point Location { get; set; }

        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        [JsonProperty("longitude")]
        public float Longitude { get; set; }

        /// <summary>
        /// Explicit cast from Document to Node object.
        /// </summary>
        /// <param name="document">Document to cast as Node</param>
        public static explicit operator Node(Document document)
        {
            return new Node(long.Parse(document.Id))
            {
                Distance = document.GetPropertyValue<double>("distance"),
                Counter = document.GetPropertyValue<int>("counter"),
                Location = document.GetPropertyValue<Point>("location"),
                Latitude = document.GetPropertyValue<float>("latitude"),
                Longitude = document.GetPropertyValue<float>("longitude")
            };
        }
    }
}
