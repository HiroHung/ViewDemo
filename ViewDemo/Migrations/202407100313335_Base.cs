namespace ViewDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Account = c.String(nullable: false, maxLength: 50),
                        OrgId = c.Int(nullable: false),
                        PasswordSalt = c.String(maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 200),
                        Telphone = c.String(nullable: false, maxLength: 50),
                        Mobile = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 100),
                        Permission = c.String(maxLength: 100),
                        Gender = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orgs", t => t.OrgId, cascadeDelete: true)
                .Index(t => t.OrgId);
            
            CreateTable(
                "dbo.Orgs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 100),
                        InitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "OrgId", "dbo.Orgs");
            DropIndex("dbo.Members", new[] { "OrgId" });
            DropTable("dbo.Orgs");
            DropTable("dbo.Members");
        }
    }
}
