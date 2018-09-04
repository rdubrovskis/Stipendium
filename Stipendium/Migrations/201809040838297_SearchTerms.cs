namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SearchTerms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchTerms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Term = c.String(),
                        TimesSearched = c.Int(nullable: false),
                        LastSearched = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Stipends");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Stipends",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        PostNr = c.Int(nullable: false),
                        County = c.String(),
                        Municipality = c.String(),
                        WebSite = c.String(),
                        ContactInfo = c.String(nullable: false),
                        OrgNr = c.String(nullable: false),
                        Description = c.String(),
                        Capital = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AcceptsApplications = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropTable("dbo.SearchTerms");
        }
    }
}
