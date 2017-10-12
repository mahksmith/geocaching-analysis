using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Geocaching
{
    public class PocketQuery
    {
        private string _gpxGeocaches;
        private string _gpxWaypoints;
        private IEnumerable<Geocache> _geocaches;
        private ZipArchive _zip;

        private enum FileType
        {
            Geocaches, Waypoints
        }

        public DateTime DateGenerated { get; internal set; }
        public short EntryCount { get; internal set; }
        public string FileSize { get; internal set; }
        public IEnumerable<Geocache> Geocaches
        {
            get
            {
                if (_geocaches != null)
                    return _geocaches;

                byte[] bytes = Encoding.ASCII.GetBytes(GpxGeocaches);
                MemoryStream stream = new MemoryStream(bytes);
                _geocaches = GPXReader.ImportGPX(stream);
                return _geocaches;
            }
        }

        public string GpxGeocaches
        {
            get
            {
                if (_gpxGeocaches != null)
                {
                    return _gpxGeocaches;
                }

                if (Zip == null) return null;
                return _gpxGeocaches = ExtractGpx(Zip, FileType.Geocaches);
            }
        }
        public string GpxWaypoints
        {
            get
            {
                if (_gpxWaypoints != null)
                {
                    return _gpxWaypoints;
                }
                if (Zip == null) return null;
                return _gpxWaypoints = ExtractGpx(Zip, FileType.Waypoints);
            }
        }

        public HttpClient HttpClient { get; internal set; }
        public string Name { get; internal set; }
        public string Url { get; internal set; }
        public Object WebsiteLock { get; internal set; }
        public ZipArchive Zip
        {
            get
            {
                if (_zip != null)
                {
                    return _zip;
                }
                if (Url == null) return null;
                lock (WebsiteLock)
                    return _zip = DownloadZipAsync(HttpClient, Url).Result;
            }

        }

        public ZipArchive DownloadZip(HttpClient httpClient, string url)
        {
            lock (WebsiteLock)
            {
                if (httpClient != null && url != null)
                {
                    Debug.WriteLine($"Downloading Pocket Query {Name}");

                    var result = httpClient.GetAsync(url);
                    if (!result.IsCanceled && result.Result != null)
                        result.Result.EnsureSuccessStatusCode();

                    return new ZipArchive(result.Result.Content.ReadAsStreamAsync().Result);
                }
            }

            return null;
        }

        public async Task<ZipArchive> DownloadZipAsync(HttpClient httpClient, string url)
        {
            if (httpClient != null && url != null)
            {
                Debug.WriteLine($"Downloading Pocket Query {Name}");

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return new ZipArchive(response.Content.ReadAsStreamAsync().Result);
                return null;
            }

            return null;
        }

        private string ExtractGpx(ZipArchive zip, FileType fileType)
        {
            if (zip != null)
            {
                foreach (ZipArchiveEntry entry in Zip.Entries)
                {
                    if (entry.Name.Contains("wpts") && !(_gpxWaypoints != null))
                    {
                        _gpxWaypoints = GetZipEntry(entry);
                    }
                    else if (!entry.Name.Contains("wpts") && !(_gpxGeocaches != null))
                    {
                        _gpxGeocaches = GetZipEntry(entry);
                    }
                }

                if (fileType.Equals(FileType.Geocaches))
                    return _gpxGeocaches;
                else
                    return _gpxWaypoints;
            }

            return null;
        }

        private string GetZipEntry(ZipArchiveEntry entry)
        {
            Debug.WriteLine($"Unzipping {Name}: {entry.Name}");

            return (new StreamReader(entry.Open())).ReadToEnd();
        }

        public void Save(SqlConnection connection)
        {
            PocketQuery dbPocketQuery = null;
            SqlCommand pqCommand = new SqlCommand("SELECT DateGenerated FROM PocketQueries WHERE Name = @name");
            pqCommand.Parameters.Add(new SqlParameter("name", this.Name));
            pqCommand.Connection = connection;
            using (SqlDataReader dr = pqCommand.ExecuteReader())
            {
                dr.Read();
                if (dr.HasRows)
                {
                    if (!(DateTime.Parse(dr[0].ToString()) < this.DateGenerated))
                    {

                        Debug.WriteLine("Throwing away " + this.Name);
                        return;
                    }
                    else
                        dbPocketQuery = new PocketQuery(); //fill this out more?
                }
            }

            //Pull All Geocaches In PocketQuery from DB
            StringBuilder sb = new StringBuilder();
            foreach (Geocache g in this.Geocaches)
                sb.Append(",'" + g.GeocacheID + "'");
            sb.Remove(0, 1);

            SqlCommand command = new SqlCommand(String.Format("SELECT GeocacheID, LastChanged FROM Geocaches WHERE GeocacheID IN ({0});", sb.ToString()))
            {
                Connection = connection
            };

            var dataReader = command.ExecuteReader();
            var geocacheDataTable = new DataTable();

            //TODO handle 1205 -- deadlock
            Boolean loadOkay = false;
            while (loadOkay != true)
            {
                try
                {
                    geocacheDataTable.Load(dataReader);
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205) //deadlock
                    {
                        Console.WriteLine("sleeping geocacheDataTable"); //todo temp
                        System.Threading.Thread.Sleep(new Random().Next(1000));
                    }
                    else throw e;
                }
                loadOkay = true;
            }

            command = new SqlCommand(String.Format("SELECT GeocacheID, ID FROM Logs WHERE GeocacheID IN ({0});", sb.ToString()))
            {
                Connection = connection
            };

            dataReader = command.ExecuteReader();
            var logDataTable = new DataTable();

            //TODO handle 1205 -- deadlock
            loadOkay = false;
            while (loadOkay != true)
            {
                try
                {
                    logDataTable.Load(dataReader);
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205) //deadlock
                    {
                        Console.WriteLine("sleeping logDataTable"); //todo temp
                        System.Threading.Thread.Sleep(new Random().Next(1000));
                    }
                    else throw e;
                }
                loadOkay = true;
            }
            logDataTable.Load(dataReader);

            SqlTransaction transaction = connection.BeginTransaction();
            GeocacheRepository repo = new GeocacheRepository(connection, transaction);
            LogRepository logRepo = new LogRepository(connection, transaction);
            PocketQueryRepository pqRepo = new PocketQueryRepository(connection, transaction);

            foreach (Geocache cache in this.Geocaches)
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
                    if (logRow != null && row != null)
                    {
                        if (DateTime.Parse(row["LastChanged"].ToString()) < log.LastChanged)
                            logRepo.Update(log);
                    }
                    else
                        logRepo.Add(log);

                }
            }

            if (dbPocketQuery != null)
                pqRepo.Update(this);
            else
                pqRepo.Add(this);

            try
            {
                transaction.Commit();
            }
            catch (Exception commit)
            {
                Console.WriteLine("Commit Exception Type: {0}", commit.GetType());
                Console.WriteLine("  Message: {0}", commit.Message);
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollback)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", rollback.GetType());
                    Console.WriteLine("  Message: {0}", rollback.Message);
                }
            }
        }
    }
}