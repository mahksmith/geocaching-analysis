using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;

namespace Geocaching
{
    public class PocketQuery
    {
        private string _gpxGeocaches;
        private string _gpxWaypoints;
        private ZipArchive _zip;

        private enum FileType
        {
            Geocaches, Waypoints
        }

        public DateTime DateGenerated { get; internal set; }
        public short EntryCount { get; internal set; }
        public string FileSize { get; internal set; }
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

        public HttpClient HttpClient { get; internal set; }
        public string Name { get; internal set; }
        public string Url { get; internal set; }
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
                Debug.WriteLine("Downloading Pocket Query " + Name);

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
            Debug.WriteLine("Unzipping " + Name + ": " + entry.Name);

            return (new System.IO.StreamReader(entry.Open())).ReadToEnd();
        }
    }
}