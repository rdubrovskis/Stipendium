namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_WebSite_2_Stipend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stipends", "WebSite", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stipends", "WebSite");
        }
    }
}
