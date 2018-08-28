namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class backtostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.Int(nullable: false));
        }
    }
}
