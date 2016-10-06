using System.IO;

namespace OsmHelperInterfaces
{
    public interface IMapHandler
    {
        void DeleteCachedMaps();
        FileStream GetOsmMap();
        void RefreshMap();
    }
}
