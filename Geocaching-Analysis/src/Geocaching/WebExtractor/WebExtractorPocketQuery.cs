using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Web.UI.HtmlControls;

namespace Geocaching.WebExtractor
{
    public class WebExtractorPocketQuery
    {
        public IEnumerable<PocketQuery> ExtractPocketQueries(WebExtractor webExtractor)
        {
            HtmlAgilityPack.HtmlDocument result = webExtractor.GetPage("/pocket/");

            //Iterate over each Pocket Query to get the download strings. The fourth one should have the "<a href"
            Debug.WriteLine("Extracting PocketQueries");
            var table = result.GetElementbyId("uxOfflinePQTable");
            if (table == null)
            {
                //TODO send web extractor status message according to website errors.
                return null;
            }


            var rows = table.ChildNodes.Where(row => row.Name.Equals("tr"));
            
            List<PocketQuery> pocketQueries = new List<PocketQuery>();

            foreach (var row in rows)
            {
                PocketQuery pocketQuery = new PocketQuery()
                {
                    HttpClient = webExtractor.Client
                };
                var columns = row.ChildNodes.Where(c => c.Name.Equals("td"));

                if (columns.Count() < 6)
                    // Last line in table is not a PocketQuery entry, but a option to delete selected Queries from website.
                    continue;

                for (int column = 0; column < columns.Count(); column++)
                {

                    switch (column)
                    {
                        case 2:
                            pocketQuery.Url = columns.ElementAt(column).ChildNodes.First(n => n.Name.Equals("a")).Attributes.First(n => n.Name.Equals("href")).Value;
                            pocketQuery.Name = columns.ElementAt(column).ChildNodes.First(n => n.Name.Equals("a")).InnerText.Trim();
                            break;
                        case 3:
                            pocketQuery.FileSize = columns.ElementAt(column).InnerText.Trim();
                            break;
                        case 4:
                            pocketQuery.EntryCount = Convert.ToInt16(columns.ElementAt(column).InnerText.Trim());
                            break;
                        case 5:
                            /* Date last generated
                             * Strings will be of the form: "31 Oct 16 (last day available)" or "01 Nov 16 (1 days remaining)"
                             */
                            pocketQuery.DateGenerated = DateTime.Parse(columns.ElementAt(column).InnerText.Trim().Substring(0, 9));
                            break;
                    }
                }
                pocketQueries.Add(pocketQuery);
            }
            return pocketQueries;
        }


        public bool QueueMyFinds(WebExtractor webExtractor)
        {
            //System.Configuration.ConfigurationManager.AppSettings[""
            HtmlAgilityPack.HtmlNode node = webExtractor
                .GetPage("/pocket/")
                .GetElementbyId("ctl00_ContentBody_PQListControl1_btnScheduleNow");

            if (node != null)
            {
                HtmlAgilityPack.HtmlAttribute attr = node.ChildAttributes("disabled").First();
                if (attr != null && !attr.Value.Equals("disabled"))
                {
                    Console.WriteLine("object is not disabled");
                }
                //TODO We will do a POST
                //webExtractor.Client.

            
            }


            return false;
        }
    }
}
