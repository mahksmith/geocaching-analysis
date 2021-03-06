﻿using Geocaching.WebExtractor;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Geocaching.Tests
{
    [TestFixture]
    class PocketQueryDownloaderTest
    {
        private WebExtractor.WebExtractor webExtractor;

        [OneTimeSetUp]
        public void Setup()
        {
            webExtractor = new WebExtractor.WebExtractor();
        }

        [Test]
        public void DownloadPocketQueryIntegrationTest()
        {
            WebExtractorPocketQuery test = new WebExtractorPocketQuery();

            System.Object websiteLock = new System.Object();
            List<PocketQuery> queries = test.ExtractPocketQueries(websiteLock).ToList();
            Assert.NotNull(queries);
            Assert.NotZero(queries.Count());

            foreach (PocketQuery pq in queries)
            {
                Assert.NotNull(pq.DateGenerated);
                Assert.NotNull(pq.EntryCount);
                Assert.NotNull(pq.FileSize);
                Assert.NotNull(pq.Name);
                Assert.NotNull(pq.Url);
                Assert.NotNull(pq.HttpClient);
            }

            //Also do integration tests unzipping and file stuff. 
            //Possibly not the ideal place to put these..
            // Only need to test one link..
            PocketQuery query = queries.ElementAt(0);

            Assert.NotNull(query.Zip);
            Assert.NotZero(query.Zip.Entries.Count);

            Assert.NotNull(query.GpxGeocaches);
            Assert.IsNotEmpty(query.GpxGeocaches);

            Assert.NotNull(query.GpxWaypoints);
            Assert.IsNotEmpty(query.GpxWaypoints);
        }
    }
}
