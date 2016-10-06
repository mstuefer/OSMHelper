using System;
using OsmHelperInterfaces;

namespace OsmHelper
{
    /// <summary>
    /// Calculates distance between two given INodes
    /// </summary>
    internal static class DistanceCalculator
    {
        /// <summary>
        /// Returns distance (Harvesine) between two given INodes
        /// </summary>
        /// <param name="srcNode"></param>
        /// <param name="dstNode"></param>
        /// <returns></returns>
        public static double GetDistance(INode srcNode, INode dstNode)
        {
            return Harvesine(srcNode.Latitude, srcNode.Longitude, dstNode.Latitude, dstNode.Longitude);
        }

        private static double Harvesine(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const int meanEarthRadius = 6371;
            var deltaLatitude = ToRadians(latitude2 - latitude1);
            var deltaLongitude = ToRadians(longitude2 - longitude1);
            latitude1 = ToRadians(latitude1);
            latitude2 = ToRadians(latitude2);

            var a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) + 
                Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2) * 
                Math.Cos(latitude1) * Math.Cos(latitude2);
            return meanEarthRadius * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
