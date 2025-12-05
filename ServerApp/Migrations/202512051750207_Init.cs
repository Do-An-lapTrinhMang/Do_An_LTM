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
                        FileName = c.String(),
                        FileSize = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        Status = c.String(),
                        User_UserID = c.Int(),
                        User_UserID1 = c.Int(),
                        Receiver_UserID = c.Int(),
                        Sender_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.HistoryID)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .ForeignKey("dbo.Users", t => t.User_UserID1)
                .ForeignKey("dbo.Users", t => t.Receiver_UserID)
                .ForeignKey("dbo.Users", t => t.Sender_UserID)
                .Index(t => t.User_UserID)
                .Index(t => t.User_UserID1)
                .Index(t => t.Receiver_UserID)
                .Index(t => t.Sender_UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        PasswordHash = c.String(),
                        FullName = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Histories", "Sender_UserID", "dbo.Users");
            DropForeignKey("dbo.Histories", "Receiver_UserID", "dbo.Users");
            DropForeignKey("dbo.Histories", "User_UserID1", "dbo.Users");
            DropForeignKey("dbo.Histories", "User_UserID", "dbo.Users");
            DropIndex("dbo.Histories", new[] { "Sender_UserID" });
            DropIndex("dbo.Histories", new[] { "Receiver_UserID" });
            DropIndex("dbo.Histories", new[] { "User_UserID1" });
            DropIndex("dbo.Histories", new[] { "User_UserID" });
            DropTable("dbo.Users");
            DropTable("dbo.Histories");
        }
    }
}
