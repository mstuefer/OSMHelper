using System.Collections.Generic;

namespace OsmHelperInterfaces
{
    public interface IWay
    {
        long Id { get; set; }
        List<long> NdsList { get; set; }
        bool OneWay { get; set; }
    }
}
