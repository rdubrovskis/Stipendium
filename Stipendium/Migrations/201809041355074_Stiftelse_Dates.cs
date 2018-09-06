namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stiftelse_Dates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stiftelses", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Stiftelses", "LastModified", c => c.DateTime());
            DropColumn("dbo.Stiftelses", "DateAdded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stiftelses", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Stiftelses", "LastModified", c => c.DateTime(nullable: false));
            DropColumn("dbo.Stiftelses", "DateCreated");
        }
    }
}
