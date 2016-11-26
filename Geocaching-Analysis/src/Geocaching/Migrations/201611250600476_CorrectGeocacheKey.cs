namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectGeocacheKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Geocaches");
            AlterColumn("dbo.Geocaches", "Name", c => c.String());
            AlterColumn("dbo.Geocaches", "Code", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Geocaches", "Code");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Geocaches");
            AlterColumn("dbo.Geocaches", "Code", c => c.String());
            AlterColumn("dbo.Geocaches", "Name", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Geocaches", "Name");
        }
    }
}
