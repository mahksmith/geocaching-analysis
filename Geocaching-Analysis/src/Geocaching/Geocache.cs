using System.Device.Location;

namespace Geocaching
{
    public class Geocache: GeoCoordinate
    {
        public string CacheID { get; internal set; }
        public string Code { get; internal set; }
        public string Country { get; internal set; }
        public string Description { get; internal set; }
        public float Difficulty { get; internal set; }
        public string LongDescription { get; internal set; }
        public string Name { get; internal set; }
        public string Owner { get; internal set; }
        public string ShortDescription { get; internal set; }
        public string Size { get; internal set; }
        public string State { get; internal set; }
        public bool StatusArchived { get; internal set; }
        public bool StatusAvailable { get; internal set; }
        public string Symbol { get; internal set; }
        public string SymbolType { get; internal set; }
        public float Terrain { get; internal set; }
        public string Time { get; internal set; }
        public string Type { get; internal set; }
        public string URL { get; internal set; }
        public string URLName { get; internal set; }
    }
}