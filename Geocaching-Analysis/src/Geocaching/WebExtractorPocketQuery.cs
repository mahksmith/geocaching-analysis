using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Geocaching
{
    public class WebExtractorPocketQuery
    {
        public List<PocketQuery> ExtractPocketQueries(HttpClient client = null)
        {
            if (client == null)
            {
                Uri baseAddress = new Uri("https://www.geocaching.com/");
                CookieContainer cookieContainer = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                client = new HttpClient(handler) { BaseAddress = baseAddress };
            }


            Debug.WriteLine("Accessing Pocket Query Login..");
            var pageResult = client.GetAsync("/account/login?returnUrl=%2Fpocket%2F");
            pageResult.Result.EnsureSuccessStatusCode();

            Debug.WriteLine("Extracting Form Information..");
            string requestVerificationToken = extractRequestVerificationToken(pageResult);

            //string needs to be of the form
            //__RequestVerificationToken=[requestVerificationToken]&Username=[username]&Password=[password]
            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("__RequestVerificationToken", requestVerificationToken),
                    new KeyValuePair<string, string>("Username", "***REMOVED***"),
                    new KeyValuePair<string, string>("Password", "***REMOVED***")
                });

            Debug.WriteLine("Logging in..");
            var task = client.PostAsync("/account/login?returnUrl=%2fpocket%2f", formContent);

            var read = task.Result.Content.ReadAsStringAsync();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(task.Result.Content.ReadAsStreamAsync().Result);

            //Iterate over each Pocket Query to get the download strings. The fourth one should have the "<a href"
            Debug.WriteLine("Extracting PocketQueries");
            var rows = doc.GetElementbyId("uxOfflinePQTable").ChildNodes.Where(row => row.Name.Equals("tr"));


            List<PocketQuery> pocketQueries = new List<PocketQuery>();

            foreach (var row in rows)
            {
                PocketQuery pocketQuery = new PocketQuery();
                pocketQuery.HttpClient = client;

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

        private string extractRequestVerificationToken(Task<HttpResponseMessage> pageResult)
        {
            //<input name="__RequestVerificationToken" type="hidden" value="[value]">
            var restest = pageResult.Result.Content.ReadAsStringAsync();
            int startIndex = restest.Result.IndexOf("<input name=\"__RequestVerificationToken");
            int endIndex = restest.Result.IndexOf('>', startIndex + 1);

            string[] tokens = restest.Result.Substring(startIndex, endIndex - startIndex).Split(' ');
            tokens = tokens[3].Split('\"');

            return tokens[1];
        }
    }
}
