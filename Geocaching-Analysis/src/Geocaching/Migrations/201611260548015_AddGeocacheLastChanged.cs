namespace Geocaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeocacheLastChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Geocaches", "LastChanged", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Geocaches", "LastChanged");
        }
    }
}
