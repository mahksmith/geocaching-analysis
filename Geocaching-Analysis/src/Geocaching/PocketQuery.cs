using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;

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
                return _gpxWaypoints = ExtractGpx(Zip, FileType.Waypoints);
            }
        }

        [NotMapped]
        public HttpClient HttpClient { get; internal set; }
        [Key]
        public string Name { get; internal set; }
        public string Url { get; internal set; }
        [NotMapped]
        public ZipArchive Zip
        {
            get
            {
                if (_zip != null)
                {
                    return _zip;
                }
                return _zip = DownloadZip(HttpClient, Url);
            }

        }

        public ZipArchive DownloadZip(HttpClient httpClient, string url)
        {
            if (httpClient != null && url != null)
            {
                Debug.WriteLine($"Downloading Pocket Query {Name}");

                var result = httpClient.GetAsync(url);
                result.Result.EnsureSuccessStatusCode();

                return new ZipArchive(result.Result.Content.ReadAsStreamAsync().Result);
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

            return (new System.IO.StreamReader(entry.Open())).ReadToEnd();
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
                    if (logRow != null)
                        logRepo.Update(log);
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