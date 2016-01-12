using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

// Package Manager Console
// ... for migrations ...
// PM> Enable-Migrations -ContextTypeName FCMAuctionDb
// s.a. Migrations folder
// ... then PM> Update-Database
// For missing Migrations/Initial.cs
// http://stackoverflow.com/questions/11679385/reset-entity-framework-migrations
// ===> PM> Add-Migration Initial

namespace FCMAuction.Models
{
    public class FCMAuctionDb : DbContext
    {
        public FCMAuctionDb() : base("name=DefaultConnection")
        {

        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemBid> ItemBids { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}