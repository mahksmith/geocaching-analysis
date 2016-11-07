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
        }
    }
}
