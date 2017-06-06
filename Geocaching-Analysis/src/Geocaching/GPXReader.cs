using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Geocaching
{
    public class GPXReader
    {
        private string path;

        private static XNamespace ns = "http://www.topografix.com/GPX/1/0",
                                  gs = "http://www.groundspeak.com/cache/1/0/1";

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
            List<Geocache> geocaches = new List<Geocache>();

            DateTime datePocketQueryGenerated = GetGenerationDate(doc);

            foreach (XElement wpt in doc.Descendants(ns + "wpt"))
            {
                Geocache geocache = new Geocache()
                {
                    Latitude = Convert.ToSingle(wpt.Attribute("lat").Value),
                    Longitude = Convert.ToSingle(wpt.Attribute("lon").Value)
                };

                geocache.LastChanged = datePocketQueryGenerated;

                geocache.Time = wpt.Element(ns + "time").Value;
                geocache.GeocacheID = wpt.Element(ns + "name").Value;
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
                foreach (XElement l in groundspeak.Descendants(gs + "log"))
                {
                    Log log = new Log(geocache);
                    log.ID = Int64.Parse(l.FirstAttribute.Value);
                    /* date */
                    log.Date = DateTime.Parse(l.Element(gs + "date").Value);
                    /* type */
                    log.Type = l.Element(gs + "type").Value;
                    /* author */
                    log.Author = l.Element(gs + "finder").Value; //TODO get finder ID from attribute
                    /* text */
                    log.Text = l.Element(gs + "text").Value;
                    /* text encoded */
                    log.TextEncoded = Boolean.Parse(l.Element(gs + "text").FirstAttribute.Value);
                    log.LastChanged = geocache.LastChanged;



                    geocache.Logs.Add(log);


                }

                geocaches.Add(geocache);
            }

            return geocaches;
        }

        public static DateTime GetGenerationDate(XDocument doc)
        {
            return DateTime.Parse(doc.Descendants(ns + "time").First().Value);
        }
    }
}
