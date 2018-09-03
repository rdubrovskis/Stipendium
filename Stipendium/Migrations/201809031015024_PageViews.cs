namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageViews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pageviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ViewCount = c.Int(nullable: false),
                        Stiftelse_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stiftelses", t => t.Stiftelse_Id)
                .Index(t => t.Stiftelse_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pageviews", "Stiftelse_Id", "dbo.Stiftelses");
            DropIndex("dbo.Pageviews", new[] { "Stiftelse_Id" });
            DropTable("dbo.Pageviews");
        }
    }
}
