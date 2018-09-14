namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Body = c.String(),
                        CommentDate = c.DateTime(nullable: false),
                        EditDate = c.DateTime(),
                        Commenter_Id = c.String(maxLength: 128),
                        Stiftelse_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Commenter_Id)
                .ForeignKey("dbo.Stiftelses", t => t.Stiftelse_Id)
                .Index(t => t.Commenter_Id)
                .Index(t => t.Stiftelse_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Stiftelse_Id", "dbo.Stiftelses");
            DropForeignKey("dbo.Comments", "Commenter_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "Stiftelse_Id" });
            DropIndex("dbo.Comments", new[] { "Commenter_Id" });
            DropTable("dbo.Comments");
        }
    }
}
