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

            //Task queriesParent = Task.Factory.StartNew(() =>
            //{
            //Check PocketQueries, save any new data.
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

            Task.WaitAll(pocketQueryTasks);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Finished, press any key (debug)");
                Console.ReadKey();
            }
        }
    }
}
