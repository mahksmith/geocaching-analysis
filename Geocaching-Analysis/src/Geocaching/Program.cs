using Geocaching.WebExtractor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Geocaching
{
    class Program
    {
        static void Main(string[] args)
        {
            WebExtractor.WebExtractor extractor = new WebExtractor.WebExtractor();
            //Check PocketQueries, save any new data.
            List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries(extractor).ToList();

            foreach (PocketQuery pocketQuery in queries)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
                    conn.Open();

                    pocketQuery.Save(conn);
                }
            }



            if (Debugger.IsAttached)
                Console.ReadLine();
        }
    }
}
