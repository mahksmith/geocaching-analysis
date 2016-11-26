namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Geocaches",
                c => new
                {
                    Name = c.String(nullable: false, maxLength: 128),
                    CacheID = c.String(),
                    Code = c.String(),
                    Country = c.String(),
                    Description = c.String(),
                    Difficulty = c.Single(nullable: false),
                    LongDescription = c.String(),
                    Owner = c.String(),
                    ShortDescription = c.String(),
                    Size = c.String(),
                    State = c.String(),
                    StatusArchived = c.Boolean(nullable: false),
                    StatusAvailable = c.Boolean(nullable: false),
                    Symbol = c.String(),
                    SymbolType = c.String(),
                    Terrain = c.Single(nullable: false),
                    Time = c.String(),
                    Type = c.String(),
                    URL = c.String(),
                    URLName = c.String(),
                    Latitude = c.Double(nullable: false),
                    Longitude = c.Double(nullable: false),
                    Altitude = c.Double(nullable: false),
                    HorizontalAccuracy = c.Double(nullable: false),
                    VerticalAccuracy = c.Double(nullable: false),
                    Speed = c.Double(nullable: false),
                    Course = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.Name);

            CreateTable(
                "dbo.PocketQueries",
                c => new
                {
                    Name = c.String(nullable: false, maxLength: 128),
                    DateGenerated = c.DateTime(nullable: false),
                    EntryCount = c.Short(nullable: false),
                    FileSize = c.String(),
                    Url = c.String(),
                })
                .PrimaryKey(t => t.Name);

        }

        public override void Down()
        {
            DropTable("dbo.PocketQueries");
            DropTable("dbo.Geocaches");
        }
    }
}
