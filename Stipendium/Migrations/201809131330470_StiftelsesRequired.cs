namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StiftelsesRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stiftelses", "Stiftelsenr", c => c.String(nullable: false));
            AlterColumn("dbo.Stiftelses", "Aktnr", c => c.String(nullable: false));
            AlterColumn("dbo.Stiftelses", "Stiftelsenamn", c => c.String(nullable: false));
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.String());
            AlterColumn("dbo.Stiftelses", "Stiftelsenamn", c => c.String());
            AlterColumn("dbo.Stiftelses", "Aktnr", c => c.String());
            AlterColumn("dbo.Stiftelses", "Stiftelsenr", c => c.String());
        }
    }
}
