namespace ServerApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Histories",
                c => new
                    {
                        HistoryID = c.Int(nullable: false, identity: true),
                        SenderID = c.Int(nullable: false),
                        ReceiverID = c.Int(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 255),
                        FileSize = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.HistoryID)
                .ForeignKey("dbo.Users", t => t.ReceiverID)
                .ForeignKey("dbo.Users", t => t.SenderID)
                .Index(t => t.SenderID)
                .Index(t => t.ReceiverID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false),
                        FullName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Histories", "SenderID", "dbo.Users");
            DropForeignKey("dbo.Histories", "ReceiverID", "dbo.Users");
            DropIndex("dbo.Histories", new[] { "ReceiverID" });
            DropIndex("dbo.Histories", new[] { "SenderID" });
            DropTable("dbo.Users");
            DropTable("dbo.Histories");
        }
    }
}
