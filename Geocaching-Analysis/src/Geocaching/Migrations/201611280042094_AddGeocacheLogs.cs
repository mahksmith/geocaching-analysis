namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeocacheLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Type = c.String(),
                        Author = c.String(),
                        Text = c.String(),
                        TextEncoded = c.Boolean(nullable: false),
                        ParentGeocache_Code = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Geocaches", t => t.ParentGeocache_Code)
                .Index(t => t.ParentGeocache_Code);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "ParentGeocache_Code", "dbo.Geocaches");
            DropIndex("dbo.Logs", new[] { "ParentGeocache_Code" });
            DropTable("dbo.Logs");
        }
    }
}
