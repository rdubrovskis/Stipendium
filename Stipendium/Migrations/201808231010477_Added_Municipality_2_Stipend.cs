namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Municipality_2_Stipend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stipends", "Municipality", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stipends", "Municipality");
        }
    }
}
