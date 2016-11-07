using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Geocaching
{
    public class GPXReader
    {
        private string path;
        public GPXReader(string path)
        {
            this.path = path;
        }

        public static IEnumerable<Geocache> ImportGPX(string path)
        {
            XDocument doc = XDocument.Load(path);

            return ImportGPX(doc);
        }

        public static IEnumerable<Geocache> ImportGPX(System.IO.Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            return ImportGPX(doc);
        }

        private static IEnumerable<Geocache> ImportGPX(XDocument doc)
        {
            XNamespace ns = "http://www.topografix.com/GPX/1/0";
            XNamespace gs = "http://www.groundspeak.com/cache/1/0/1";

            List<Geocache> geocaches = new List<Geocache>();

            foreach (XElement wpt in doc.Descendants(ns + "wpt"))
            {
                Geocache geocache = new Geocache();

                geocache.Latitude = Convert.ToSingle(wpt.Attribute("lat").Value);
                geocache.Longitude = Convert.ToSingle(wpt.Attribute("lon").Value);
                geocache.GetDistanceTo(geocache);

                geocache.Time = wpt.Element(ns + "time").Value;
                geocache.Code = wpt.Element(ns + "name").Value;
                geocache.Description = wpt.Element(ns + "desc").Value;
                geocache.URL = wpt.Element(ns + "url").Value;
                geocache.URLName = wpt.Element(ns + "urlname").Value;
                geocache.Symbol = wpt.Element(ns + "sym").Value;
                geocache.SymbolType = wpt.Element(ns + "type").Value;

                XElement groundspeak = wpt.Element(gs + "cache");

                geocache.CacheID = groundspeak.Attribute("id").Value;
                geocache.StatusAvailable = Convert.ToBoolean(groundspeak.Attribute("available").Value);
                geocache.StatusArchived = Convert.ToBoolean(groundspeak.Attribute("archived").Value);
                geocache.Name = groundspeak.Element(gs + "name").Value;
                geocache.Owner = groundspeak.Element(gs + "owner").Value;
                geocache.Type = groundspeak.Element(gs + "type").Value;
                geocache.Size = groundspeak.Element(gs + "container").Value;

                //TODO:attributes go here


                geocache.Difficulty = Convert.ToSingle(groundspeak.Element(gs + "difficulty").Value);
                geocache.Terrain = Convert.ToSingle(groundspeak.Element(gs + "terrain").Value);
                geocache.Country = groundspeak.Element(gs + "country").Value;
                geocache.State = groundspeak.Element(gs + "state").Value;
                geocache.ShortDescription = groundspeak.Element(gs + "short_description").Value;
                geocache.LongDescription = groundspeak.Element(gs + "long_description").Value;

                // save logs here

                geocaches.Add(geocache);
            }

            return geocaches;
        }
    }
}
