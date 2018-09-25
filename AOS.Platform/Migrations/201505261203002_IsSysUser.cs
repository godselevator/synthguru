namespace AOS.Platform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSysUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsSysUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsSysUser");
        }
    }
}
