namespace FCMAuction.Migrations
{
    using FCMAuction.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<FCMAuction.Models.FCMAuctionDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //ContextKey = "FCMAuction.Models.FCMAuctionDb";
        }

        // How do you update the db?
        // 1.  configure app to automatically apply updates
        // 2.  PM> Update-Database -Verbose
        //
        // ... or
        // delete the existing DB tables (Items, ItemBids, UserProfile
        // delete rows associated with _Roles and _UsersInRoles
        // remove Migrations/Initial_...
        // then PM> Update-Database -Verbose
        protected override void Seed(FCMAuction.Models.FCMAuctionDb context)
        {
            //context.Items.AddOrUpdate(
            //    r => r.Name,
            //    new Item
            //    {
            //        Name = "Fist Item",
            //        Description = "First Description...",
            //        Image = "1.jpg",
            //        Value = 42,
            //        MinimumBid = 43,
            //        Bids = new List<ItemBid> { 
            //            new ItemBid { Bid = 44, ItemId = 1, UserId = 1}
            //         }
            //    });


            SeedMembership();
        }

        // PS-MVC4 /  Security and ASP.NET 4 / Seeding Membership
        // PS-MVC4 / Deployment and Configuration / Preparing for Deployment
        // Update-Database Error: The Role Manager feature has not been enabled.
        // Need to modify web.config
        private void SeedMembership()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (membership.GetUser("fcm", false) == null)
            {
                membership.CreateUserAndAccount("fcm", "fcm_password");
            }
            if (!roles.GetRolesForUser("fcm").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "fcm" }, new[] { "admin" });
            }
        }
    }
}
