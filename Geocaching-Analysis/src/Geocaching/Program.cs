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

        private static void SavePocketQuery(SqlConnection connection, PocketQuery pocketQuery)
        {

            SqlCommand pqCommand = new SqlCommand("SELECT COUNT(*) FROM PocketQueries WHERE PocketQueries.DateGenerated < @date AND PocketQueries.Name = @name");
            pqCommand.Parameters.Add(new SqlParameter("date", pocketQuery.DateGenerated));
            pqCommand.Parameters.Add(new SqlParameter("name", pocketQuery.Name));
            pqCommand.Connection = connection;
            using (SqlDataReader dr = pqCommand.ExecuteReader())
            {
                dr.Read();
                if (int.Parse(dr[0].ToString()) > 0) { }

                else
                {
                    Debug.WriteLine("Throwing away " + pocketQuery.Name);
                    return;
                }
            }

            //Pull All Geocaches In PocketQuery from DB
            StringBuilder sb = new StringBuilder();
            foreach (Geocache g in pocketQuery.Geocaches)
                sb.Append(",'" + g.GeocacheID + "'");
            sb.Remove(0, 1);

            SqlCommand command = new SqlCommand("SELECT GeocacheID, LastChanged FROM Geocaches WHERE GeocacheID IN (" + sb.ToString() + ");")
            {
                Connection = connection
            };

            var dataReader = command.ExecuteReader();
            var geocacheDataTable = new DataTable();
            geocacheDataTable.Load(dataReader);

            command = new SqlCommand("SELECT GeocacheID, ID FROM Logs WHERE GeocacheID IN (" + sb.ToString() + ");")
            {
                Connection = connection
            };

            dataReader = command.ExecuteReader();
            var logDataTable = new DataTable();
            logDataTable.Load(dataReader);

            SqlTransaction transaction = connection.BeginTransaction();
            GeocacheRepository repo = new GeocacheRepository(connection, transaction);

            foreach (Geocache cache in pocketQuery.Geocaches)
            {
                DataRow row = geocacheDataTable.Select("GeocacheID = '" + cache.GeocacheID + "'").FirstOrDefault();
                if (row != null)
                {
                    if (DateTime.Parse(row["LastChanged"].ToString()) < cache.LastChanged)
                    {
                        repo.Update(cache);
                    }
                }
                else
                {
                    //Add to database
                    repo.Add(cache);
                }

                //Also save logs in each geocache.
                foreach (Log log in cache.Logs)
                {
                    DataRow logRow = logDataTable.Select("GeocacheID = '" + log.GeocacheID + "' AND " + "ID = '" + log.ID + "'").FirstOrDefault();
                    if (logRow != null)
                    {
                        //logRepo.Update(log); TODO
                    }
                    else
                    {
                        //logRepo.Add(log); TODO
                    }
                }
            }


            //Also save pocketQuery
            

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
