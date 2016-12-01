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

            //Check PocketQueries, save any new data.
            List<PocketQuery> queries = new WebExtractorPocketQuery().ExtractPocketQueries().ToList();

            foreach (PocketQuery pocketQuery in queries)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Geocaching.DatabaseEntityContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    conn.Open();

                    SavePocketQuery(conn, pocketQuery);
                }
            }
        }

        private static void SavePocketQuery(SqlConnection conn, PocketQuery pocketQuery)
        {
            //There's no point download Pocket Queries that have not been updated.
            //TODO: Try to find if theres a different way to check if Pocket Queries differ without downloading?

            // Pocket Query update using http://www.entityframeworktutorial.net/EntityFramework4.3/update-entity-using-dbcontext.aspx

            PocketQuery pq = null;

            SqlCommand pqCommand = new SqlCommand("SELECT COUNT(*) FROM PocketQueries WHERE PocketQueries.DateGenerated < @date AND PocketQueries.Name = @name");
            pqCommand.Parameters.Add(new SqlParameter("date", pocketQuery.DateGenerated));
            pqCommand.Parameters.Add(new SqlParameter("name", pocketQuery.Name));
            pqCommand.Connection = conn;
            using (SqlDataReader dataReader = pqCommand.ExecuteReader())
            {
                dataReader.Read();
                if (int.Parse(dataReader[0].ToString()) > 0) { }

                else
                {
                    Debug.WriteLine("Throwing away " + pocketQuery.Name);
                    return;
                }
            }

            //Build Delta Query.
            StringBuilder sb = new StringBuilder();
            foreach (Geocache g in pocketQuery.Geocaches)
                sb.Append(",'" + g.GeocacheID + "'");
            sb.Remove(0, 1);
            
            SqlCommand command = new SqlCommand("SELECT GeocacheID, LastChanged FROM Geocaches WHERE GeocacheID IN (" + sb.ToString() + ");")
            {
                Connection = conn
            };

            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand saveGeocache = conn.CreateCommand();
                saveGeocache.Transaction = transaction;
                
                while (dataReader.Read()) {
                    Geocache toBeUpdated = pocketQuery.Geocaches.Single(a=> a.GeocacheID.Equals(dataReader["GeocacheID"].ToString()));
                    DateTime lastchanged = DateTime.Parse(dataReader["LastChanged"].ToString());
                    if (lastchanged < toBeUpdated.LastChanged)
                    {
                        command.CommandText = "UPDATE Geocaches SET Description = 'foobar' WHERE GeocacheID IN ('" + toBeUpdated.CacheID + "');";
                        command.ExecuteNonQuery();
                    }
                }


                //Also save pocketQuery.



                transaction.Commit();
            }



            //2. change stuff?
            //if (pq != null && pq.DateGenerated.Equals(pocketQuery.DateGenerated))
            //{
            //    Debug.WriteLine("Throwing away " + pocketQuery.Name);
            //    return;
            //}

            ////TODO I've selected ALL geocache codes, there must be a way to get only those in the PQ.
            //var databaseTest = ctx.Geocaches.AsNoTracking().Where(a => true).ToDictionary(a => a.GeocacheID);
            //foreach (Geocache cache in pocketQuery.Geocaches)
            //{
            //    if (databaseTest.ContainsKey(cache.GeocacheID))
            //    {
            //        ctx.Geocaches.Attach(cache);
            //        ctx.Entry(cache).State = System.Data.Entity.EntityState.Modified;
            //    }
            //    else
            //    {
            //        ctx.Geocaches.Add(cache);
            //    }

            //    foreach (Log log in cache.Logs)
            //    {
            //        if (databaseTest.ContainsKey(cache.GeocacheID) && databaseTest[cache.GeocacheID].Logs.SingleOrDefault(a => a.ID == log.ID) != null)
            //        {
            //            ctx.Logs.Attach(log);
            //            ctx.Entry(log).State = System.Data.Entity.EntityState.Modified;
            //        }
            //        else
            //            ctx.Logs.Add(log);
            //    }
            //}

            //if (pq != null)
            //{
            //    ctx.PocketQueries.Attach(pocketQuery);
            //    ctx.Entry(pocketQuery).State = System.Data.Entity.EntityState.Modified;
            //}
            //else
            //{
            //    ctx.PocketQueries.Add(pocketQuery);
            //}

            //ctx.SaveChanges();
        }
    }
}
