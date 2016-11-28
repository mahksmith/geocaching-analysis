using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Device.Location;

namespace Geocaching
{
    public class Geocache : GeoCoordinate
    {
        public Geocache()
        {
            //Future: maybe we can extract altitudes from Google Maps? Would need to do it if coordinates ever update.
            Altitude = 0;

            Logs = new List<Log>();
        }

        public string CacheID { get; internal set; }
        [Key]
        public string Code { get; internal set; }
        public string Country { get; internal set; }
        public string Description { get; internal set; }
        public float Difficulty { get; internal set; }
        public DateTime LastChanged { get; internal set; }
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

        public virtual ICollection<Log> Logs { get; internal set; }
    }

    public class Log
    {
        [Key, DatabaseG‌​enerated(DatabaseGen‌​eratedOption.None)]
        public Int64 ID { get; internal set; }
        public virtual Geocache ParentGeocache { get; internal set; }
        public DateTime Date { get; internal set; }
        public String Type { get; internal set; }
        public String Author { get; internal set; }
        public String Text { get; internal set; }
        public Boolean TextEncoded { get; internal set; }
        public Log(Geocache parent)
        {
            ParentGeocache = parent;
        }
    }
}