using System.Collections.Generic;
using OsmHelperInterfaces;

namespace OsmHelper
{
    /// <summary>
    /// Represents the Way of the osm xml.
    /// </summary>
    public class Way : IWay
    {
        public Way()
        {
            NdsList = new List<long>();
        }

        /// <summary>
        /// The Id of the Way in the osm xml.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The list of referenced nodes (Called ND in the osm xml).
        /// </summary>
        public List<long> NdsList { get; set; }
        /// <summary>
        /// Set to true if way allows traversing only in one direction.
        /// </summary>
        public bool OneWay { get; set; }
    }
}
