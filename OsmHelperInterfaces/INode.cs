namespace OsmHelperInterfaces
{
    public interface INode
    {
        long Id { get; set; }
        double Distance { get; set; } // in km
        int Counter { get; set; }
        float Latitude { get; set; }
        float Longitude { get; set; }
    }
}
