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

        // Objects for processing and events.
        static void Main(string[] args)
        {

            List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries(websiteLock).ToList();

            Task[] pocketQueryTasks = new Task[queries.Count];

            for (int i = 0; i < pocketQueryTasks.Length; i++)
            {
                int j = i;
                pocketQueryTasks[j] = Task.Factory.StartNew(() =>
                {
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
                        conn.Open();
                        queries[j].Save(conn);

                    }
                });
            }

            try
            {
                /* TODO:
                 * Catch System.AggregateException and log all inner exceptions
                 */
                Task.WaitAll(pocketQueryTasks);
            } catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Finished, press any key (debug)");
                Console.ReadKey();
            }
        }
    }
}
