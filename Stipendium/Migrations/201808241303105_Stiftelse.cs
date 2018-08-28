namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stiftelse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stiftelses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Stiftelsenr = c.Int(nullable: false),
                        Aktnr = c.String(),
                        Orgnr = c.String(),
                        Län = c.String(),
                        Stiftelsenamn = c.String(),
                        Kommun = c.String(),
                        Adress = c.String(),
                        CoAdress = c.String(),
                        Postnr = c.String(),
                        Postadress = c.String(),
                        Telefon = c.String(),
                        Stiftelsetyp = c.String(),
                        Status = c.String(),
                        År = c.String(),
                        Förmögenhet = c.String(),
                        Ändamål = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Stiftelses");
        }
    }
}
