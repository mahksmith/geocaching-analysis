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
            PocketQuery dbPocketQuery = null;
            SqlCommand pqCommand = new SqlCommand("SELECT DateGenerated FROM PocketQueries WHERE Name = @name");
            pqCommand.Parameters.Add(new SqlParameter("name", pocketQuery.Name));
            pqCommand.Connection = connection;
            using (SqlDataReader dr = pqCommand.ExecuteReader())
            {
                dr.Read();
                if (dr.HasRows)
                {
                    if (!(DateTime.Parse(dr[0].ToString()) < pocketQuery.DateGenerated))
                    {

                        Debug.WriteLine("Throwing away " + pocketQuery.Name);
                        return;
                    }
                    else
                        dbPocketQuery = new PocketQuery(); //fill this out more?
                }
            }

            //Pull All Geocaches In PocketQuery from DB
            StringBuilder sb = new StringBuilder();
            foreach (Geocache g in pocketQuery.Geocaches)
                sb.Append(",'" + g.GeocacheID + "'");
            sb.Remove(0, 1);

            SqlCommand command = new SqlCommand(String.Format("SELECT GeocacheID, LastChanged FROM Geocaches WHERE GeocacheID IN ({0});", sb.ToString()))
            {
                Connection = connection
            };

            var dataReader = command.ExecuteReader();
            var geocacheDataTable = new DataTable();
            geocacheDataTable.Load(dataReader);

            command = new SqlCommand(String.Format("SELECT GeocacheID, ID FROM Logs WHERE GeocacheID IN ({0});", sb.ToString()))
            {
                Connection = connection
            };

            dataReader = command.ExecuteReader();
            var logDataTable = new DataTable();
            logDataTable.Load(dataReader);

            SqlTransaction transaction = connection.BeginTransaction();
            GeocacheRepository repo = new GeocacheRepository(connection, transaction);
            LogRepository logRepo = new LogRepository(connection, transaction);
            PocketQueryRepository pqRepo = new PocketQueryRepository(connection, transaction);

            foreach (Geocache cache in pocketQuery.Geocaches)
            {
                DataRow row = geocacheDataTable.Select(String.Format("GeocacheID = '{0}'", cache.GeocacheID)).FirstOrDefault();
                if (row != null)
                {
                    if (DateTime.Parse(row["LastChanged"].ToString()) < cache.LastChanged)
                        repo.Update(cache);
                }
                else
                    repo.Add(cache);

                //Also save logs in each geocache.
                foreach (Log log in cache.Logs)
                {
                    DataRow logRow = logDataTable.Select(String.Format("GeocacheID = '{0}' AND ID = '{1}'", log.GeocacheID, log.ID)).FirstOrDefault();
                    if (logRow != null)
                        logRepo.Update(log);
                    else
                        logRepo.Add(log);
                }
            }

            if (dbPocketQuery != null)
                pqRepo.Update(pocketQuery);
            else
                pqRepo.Add(pocketQuery);

            try
            {
                transaction.Commit();
            }
            catch (Exception commit)
            {
                //Commit failed, try rollback
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollback)
                {
                    //TODO
                }

                throw;
            }
        }
    }
}
