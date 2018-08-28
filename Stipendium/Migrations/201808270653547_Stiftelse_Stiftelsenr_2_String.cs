namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stiftelse_Stiftelsenr_2_String : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stiftelses", "Stiftelsenr", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stiftelses", "Stiftelsenr", c => c.Int(nullable: false));
        }
    }
}
