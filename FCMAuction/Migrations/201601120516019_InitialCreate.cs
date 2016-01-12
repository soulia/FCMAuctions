namespace FCMAuction.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemBids",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Bid = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        Value = c.Int(nullable: false),
                        MinimumBid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemBids", "ItemId", "dbo.Items");
            DropIndex("dbo.ItemBids", new[] { "ItemId" });
            DropTable("dbo.UserProfile");
            DropTable("dbo.Items");
            DropTable("dbo.ItemBids");
        }
    }
}
