using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geocaching.Tests
{
    [TestFixture]
    class GPXReaderTest
    {
        [Test]
        public void ReadFile()
        {
            List<Geocache> geocaches = GPXReader.ImportGPX(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestContent", "TestGPX.gpx")).ToList();
            Assert.AreEqual(1000, geocaches.Count);

            foreach (Geocache cache in geocaches)
            {
                Assert.IsNotNull(cache.CacheID);
                Assert.IsNotEmpty(cache.CacheID);
                Assert.IsNotNull(cache.Code);
                Assert.IsNotEmpty(cache.Code);
                Assert.IsNotNull(cache.Country);
                Assert.IsNotEmpty(cache.Country);
                Assert.IsNotNull(cache.Description);
                Assert.IsNotEmpty(cache.Description);
                Assert.IsNotNull(cache.Difficulty);
                Assert.IsNotNull(cache.LastChanged);
                Assert.IsNotNull(cache.LongDescription);
                Assert.IsNotNull(cache.Name);
                Assert.IsNotEmpty(cache.Name);
                Assert.IsNotNull(cache.Owner);
                Assert.IsNotEmpty(cache.Owner);
                Assert.IsNotNull(cache.ShortDescription);
                Assert.IsNotNull(cache.Size);
                Assert.IsNotEmpty(cache.Size);
                Assert.IsNotNull(cache.State);
                Assert.IsNotEmpty(cache.State);
                Assert.IsNotNull(cache.StatusArchived);
                Assert.IsNotNull(cache.StatusAvailable);
                Assert.IsNotNull(cache.Symbol);
                Assert.IsNotEmpty(cache.Symbol);
                Assert.IsNotNull(cache.SymbolType);
                Assert.IsNotEmpty(cache.SymbolType);
                Assert.IsNotNull(cache.Terrain);
                Assert.IsNotNull(cache.Time);
                Assert.IsNotEmpty(cache.Time);
                Assert.IsNotNull(cache.Type);
                Assert.IsNotEmpty(cache.Type);
                Assert.IsNotNull(cache.URL);
                Assert.IsNotEmpty(cache.URL);
                Assert.IsNotNull(cache.URLName);
                Assert.IsNotEmpty(cache.URLName);

                Assert.IsTrue(cache.Logs.Count > 0);

                foreach (Log log in cache.Logs)
                {
                    Assert.IsNotNull(log.ID);
                    Assert.NotZero(log.ID);
                    Assert.IsNotNull(log.Date);
                    Assert.IsNotNull(log.Type);
                    Assert.IsNotEmpty(log.Type);
                    Assert.IsNotNull(log.Author);
                    Assert.IsNotEmpty(log.Author);
                    Assert.IsNotEmpty(log.Text);
                    Assert.IsNotNull(log.Text);
                    Assert.IsNotNull(log.TextEncoded);
                }
            }
        }
    }
}
