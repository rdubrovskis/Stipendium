namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stiftelse_capital_string_2_double : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stiftelses", "Förmögenhet", c => c.String());
        }
    }
}
