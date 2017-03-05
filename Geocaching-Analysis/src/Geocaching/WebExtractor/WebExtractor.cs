using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Geocaching.WebExtractor
{
    public class WebExtractor
    {
        private HttpClient _client;
        private HtmlDocument _currentPage;

        public HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    Uri baseAddress = new Uri("https://www.geocaching.com/");
                    CookieContainer cookieContainer = new CookieContainer();
                    HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    _client = new HttpClient(handler) { BaseAddress = baseAddress };
                }
                return _client;
            }
        }

        public HtmlDocument GetPage(string url)
        {
            _currentPage = this.RetrievePage(url);
            return _currentPage;
        }

        private HtmlDocument RetrievePage(string url)
        {
            Debug.WriteLine("Accessing Geocaching Page..");
            var task = Client.GetAsync(url);
            task.Result.EnsureSuccessStatusCode();

            /* Check if already logged in */
            Debug.WriteLine("Checking Login..");
            HtmlDocument document = new HtmlDocument();
            document.Load(task.Result.Content.ReadAsStreamAsync().Result);
            string result = document.DocumentNode.OuterHtml;
            if (result.Contains("isLoggedIn: true"))
            {
                Debug.WriteLine("Already logged in..");
                return document;
            }

            /* Not logged in, so need to navigate to logon page from here */
            Debug.WriteLine("Logging in..");

            string logInPage = GetLogInURL(document);

            //Navigate to LogIn Page
            if (logInPage.Equals(String.Empty))
            {
                Debug.WriteLine("Could not find LogIn URL. Please report this!");
                return null;
            }
            else
            {
                return LogIn(logInPage);
            }
        }

        private string GetLogInURL(HtmlDocument document)
        {
            //Search page for an "<a href"  that contains "Log In" as the text, get the URL.
            HtmlNode logInNode = document.GetElementbyId("hlSignIn");
            string logInPage = String.Empty;

            if (logInNode != null)
            {
                logInPage = logInNode.ChildAttributes("href").FirstOrDefault().Value;
            }
            if (logInNode == null)
            {

                //TODO: also add all other ways to get the log in URL..
            }

            return logInPage;
        }

        public HtmlDocument LogIn(string logInPage)
        {
            var task = Client.GetAsync(logInPage);
            task.Result.EnsureSuccessStatusCode();


            string requestVerificationToken = ExtractRequestVerificationToken(task);

            //string needs to be of the form
            //__RequestVerificationToken=[requestVerificationToken]&Username=[username]&Password=[password]
            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("__RequestVerificationToken", requestVerificationToken),
                    new KeyValuePair<string, string>("Username", System.Configuration.ConfigurationManager.AppSettings["GeocachingUsername"]),
                    new KeyValuePair<string, string>("Password", System.Configuration.ConfigurationManager.AppSettings["GeocachingPassword"])
            });

            Debug.WriteLine("Actually Logging In..");
            task = Client.PostAsync(logInPage, formContent);
            var read = task.Result.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(task.Result.Content.ReadAsStreamAsync().Result);

            return doc;
        }

        internal List<KeyValuePair<string, string>> GetCurrentViewStates()
        {
            List<KeyValuePair<string, string>> formKeyValues = new List<KeyValuePair<string, string>>
            {
                CreateKeyValuePairForElementValue("__EVENTTARGET"),
                CreateKeyValuePairForElementValue("__EVENTARGUMENT"),
                CreateKeyValuePairForElementValue("__VIEWSTATEFIELDCOUNT")
            };
            int viewStateFieldCountN = Convert.ToInt32(GetElementValueFromCurrentPage("__VIEWSTATEFIELDCOUNT"));

            List<HtmlNode> viewStates = new List<HtmlNode>();
            for (int i = 0; i < viewStateFieldCountN; i++)
            {
                if (i == 0)
                    formKeyValues.Add(CreateKeyValuePairForElementValue("__VIEWSTATE"));
                else
                    formKeyValues.Add(CreateKeyValuePairForElementValue($"__VIEWSTATE{i}"));
            }

            formKeyValues.Add(CreateKeyValuePairForElementValue("__VIEWSTATEGENERATOR"));

            return formKeyValues;
        }
        
        private KeyValuePair<string, string> CreateKeyValuePairForElementValue(string ID)
        {
            return new KeyValuePair<string, string>(ID, GetElementValueFromCurrentPage(ID));
        }

        private string GetElementValueFromCurrentPage(string ID)
        {
            return _currentPage.GetElementbyId(ID).ChildAttributes("value").First().Value;
        }

        private string ExtractRequestVerificationToken(Task<HttpResponseMessage> pageResult)
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

