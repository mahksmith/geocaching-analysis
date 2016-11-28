namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeocacheLogs : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Geocaches");
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        ID = c.Long(nullable: false),
                        GeocacheID = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Type = c.String(),
                        Author = c.String(),
                        Text = c.String(),
                        TextEncoded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Geocaches", t => t.GeocacheID)
                .Index(t => t.GeocacheID);
            
            AddColumn("dbo.Geocaches", "GeocacheID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Geocaches", "GeocacheID");
            DropColumn("dbo.Geocaches", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Geocaches", "Code", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Logs", "GeocacheID", "dbo.Geocaches");
            DropIndex("dbo.Logs", new[] { "GeocacheID" });
            DropPrimaryKey("dbo.Geocaches");
            DropColumn("dbo.Geocaches", "GeocacheID");
            DropTable("dbo.Logs");
            AddPrimaryKey("dbo.Geocaches", "Code");
        }
    }
}
