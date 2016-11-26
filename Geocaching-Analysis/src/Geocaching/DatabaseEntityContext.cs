using System.Data.Entity;

namespace Geocaching
{
    class DatabaseEntityContext: DbContext
    {
        public DatabaseEntityContext(): base()
        {

        }

        public DbSet<Geocache> Geocaches { get; set; }
        public DbSet<PocketQuery> PocketQueries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<Geocache>();
            config.Ignore(u => u.HorizontalAccuracy);
            config.Ignore(u => u.VerticalAccuracy);
            config.Ignore(u => u.Speed);
            config.Ignore(u => u.Course);
        }
    }
}
