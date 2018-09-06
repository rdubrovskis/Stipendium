namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastModified_2_Stiftelse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stiftelses", "LastModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stiftelses", "LastModified");
        }
    }
}
