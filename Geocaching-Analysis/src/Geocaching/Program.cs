using Geocaching.WebExtractor;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Geocaching
{
    class Program
    {
        private static Object websiteLock = new Object();
        static void Main(string[] args)
        {
        
            Task queriesParent = Task.Factory.StartNew(() =>
            {
                //Check PocketQueries, save any new data.
                List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries(websiteLock).ToList();

                Parallel.ForEach<PocketQuery>(queries, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, pocketQuery =>
                {
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
                        conn.Open();
                        pocketQuery.Save(conn);
                    }
                });
            });

            queriesParent.Wait();

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Finished, press any key (debug)");
                Console.ReadKey();
            }
        }
    }
}
