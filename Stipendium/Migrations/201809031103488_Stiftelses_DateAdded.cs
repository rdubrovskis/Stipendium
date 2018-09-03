namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stiftelses_DateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stiftelses", "DateAdded", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stiftelses", "DateAdded");
        }
    }
}
