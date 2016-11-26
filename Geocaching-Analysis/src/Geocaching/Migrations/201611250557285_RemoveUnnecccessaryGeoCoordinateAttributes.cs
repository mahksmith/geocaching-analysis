namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUnnecccessaryGeoCoordinateAttributes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Geocaches", "HorizontalAccuracy");
            DropColumn("dbo.Geocaches", "VerticalAccuracy");
            DropColumn("dbo.Geocaches", "Speed");
            DropColumn("dbo.Geocaches", "Course");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Geocaches", "Course", c => c.Double(nullable: false));
            AddColumn("dbo.Geocaches", "Speed", c => c.Double(nullable: false));
            AddColumn("dbo.Geocaches", "VerticalAccuracy", c => c.Double(nullable: false));
            AddColumn("dbo.Geocaches", "HorizontalAccuracy", c => c.Double(nullable: false));
        }
    }
}
