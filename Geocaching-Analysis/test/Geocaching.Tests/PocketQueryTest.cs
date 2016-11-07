using NUnit.Framework;

namespace Geocaching.Tests
{
    [TestFixture]
    public class PocketQueryTest
    {
        [Test]
        public void GetGeocachesDoesNotException()
        {
            string caches = (new PocketQuery()).GpxGeocaches;
            Assert.Null(caches);
        }

        [Test]
        public void GetWaypointsDoesNotException()
        {
            string waypoints = (new PocketQuery()).GpxWaypoints;
            Assert.Null(waypoints);
        }

        [Test]
        public void GetZipDoesNotException()
        {
            object zip = (new PocketQuery()).Zip;
            Assert.Null(zip);
        }
    }
}