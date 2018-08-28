namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class integer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
