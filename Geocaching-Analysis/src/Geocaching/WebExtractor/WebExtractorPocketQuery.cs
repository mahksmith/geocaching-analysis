using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using HtmlAgilityPack;

namespace Geocaching.WebExtractor
{
    public class WebExtractorPocketQuery
    {
        public IEnumerable<PocketQuery> ExtractPocketQueries(Object websiteLock)
        {
            WebExtractor webExtractor = new WebExtractor();
            HtmlDocument result = webExtractor.GetPage("/pocket/");

            //Determine if log in was successful
            //<div class="validation-summary-errors">

            //TODO When GUI is implemented, need to write this error to GUI message, or debug.
            var loginResult = result.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") &&
                d.Attributes["class"].Value.Equals("validation-summary-errors"));

            if (loginResult != null)
            {
                Debug.WriteLine("Password or Email incorrect!");
                return new List<PocketQuery>();
            }

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
                pocketQuery.WebsiteLock = websiteLock;
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
            HtmlDocument website = webExtractor.GetPage("/pocket/default.aspx");
            HtmlNode node = website
                .GetElementbyId("ctl00_ContentBody_PQListControl1_btnScheduleNow");

            if (node != null)
            {
                HtmlAgilityPack.HtmlAttribute attr = node.ChildAttributes("disabled").FirstOrDefault();

                if (attr == null || !attr.Value.Equals("disabled"))
                {
                    // Button is not disabled. Need to copy Viewstate information across. Create the formContent and Post.
                    List<KeyValuePair<string, string>> formContent = webExtractor.GetCurrentViewStates();

                    formContent.Add(new KeyValuePair<string, string>("ctl00$ContentBody$PQListControl1$btnScheduleNow", "Add to Queue"));
                    formContent.Add(new KeyValuePair<string, string>("ctl00$ContentBody$PQListControl1$hidIds", String.Empty));
                    formContent.Add(new KeyValuePair<string, string>("ctl00$ContentBody$PQDownloadList$hidIds", String.Empty));

                    var formURLEncodedContent = new System.Net.Http.FormUrlEncodedContent(formContent);
                    
                    var task = webExtractor.Client.PostAsync("/pocket/default.aspx", formURLEncodedContent);
                    var read = task.Result.Content.ReadAsStringAsync().Result;

                    //Need to check for success message!
                    //<p class="Success">Your 'My Finds' Pocket Query has been scheduled to run.</p>
                    //HtmlNode successMessage = website.DocumentNode.SelectNodes(String.Format("//p[@class='{0}']", "Success")).FirstOrDefault();
                    //if (successMessage != null)
                    //{
                    //    return true;
                    //}

                    //TODO: Possibly also add this to the queue in three days time..
                }
            }

            return false;
        }
    }
}
