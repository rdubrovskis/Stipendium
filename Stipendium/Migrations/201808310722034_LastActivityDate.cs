namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastActivityDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastActivityDate", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastActivityDate");
        }
    }
}
