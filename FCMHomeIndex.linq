<Query Kind="Statements">
  <Connection>
    <ID>0d97ad61-b912-4494-bbe1-08b721ad6bd2</ID>
    <Persist>true</Persist>
    <Server>tcp:fcm.database.windows.net,1433</Server>
    <SqlSecurity>true</SqlSecurity>
    <UserName>soulia@fcm</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAL4fxIlcFI0qbPWKa6NYZlQAAAAACAAAAAAAQZgAAAAEAACAAAAARqGr32x3gfeJNGkdzGe820CJPIJivhs4luDw8RDZdwgAAAAAOgAAAAAIAACAAAABomdfXlBbNffA4wZIhmE4VHV77Y4om2J7f04ZQZ5YM8hAAAAD/o3165xqZR8z2QAd8oiNXQAAAADfndWZ6LIFAW6aTdlRIF9f4afsaElfx2lyp39KeEh0I/lho4ZymVvEZNIsE1S+qmwCORaGnlxMPufNo8wtydN0=</Password>
    <DbVersion>Azure</DbVersion>
    <Database>fcm</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

//==========================================================================
// Home / Index

	// This is the original model query for the HomeController - 1/26/2016
  //var model = Items
  /******************
  var itemBids = Items
      //.OrderByDescending(r => r.Bids.Max(bid => bid.Bid))
      .OrderBy(r => r.Name)
      .Select(r => new 
      {
          Id = r.Id,
          Name = r.Name,
          Description = r.Description,
          Image = r.Image,
          Value = r.Value,
          MinimumBid = r.MinimumBid,
          NewBid = r.MinimumBid//,
          // http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
          //HighestBid = //(int?)r.Bids.Max(b => b.Bid) ?? 0 
      });
	  
Console.WriteLine(itemBids);
****************/

// broken - 
//var winningBids = itemBids.GroupBy(i => i.Id).Select(g => new {g.Key, Bid = g.Max(b => b.Bid)});
//Console.WriteLine(winningBids);

// Here's the new query 1/27/2016
// now with left join...
// http://stackoverflow.com/questions/584820/how-do-you-perform-a-left-outer-join-using-linq-extension-methods

/**************
/// ... The following shows just the Bids. Items with NO BIDS are excluded. DO NOT USE THIS FOR THE Home / Index page.
var allItemBids = ItemBids.GroupJoin(Items, 
						ib => ib.ItemId, 
						i => i.Id,
						(x,y) => new 
							{ ItemBids = x, Items = y })
						.SelectMany(
							x => x.Items.DefaultIfEmpty(),
							(x, y) => new {ItemBids = x.ItemBids, Items = y});							
// 406 ItemBids NOTE!!! This does not count the Items with No Bids. There are currently 406 bids with 10 items that have no bids as of 1/29/16
***********/

/// Show all ItemBids for each Item - new model for HomeController - 1/27/2016
/// "include all Items and show all ItemBids on Items"
/// We WANT to show ALL Items on the Home / Index page
var allItemsBidHistory = Items.GroupJoin(ItemBids, 
						i => i.Id, 
						ib => ib.ItemId,
						(x,y) => new 
							{ Items = x, ItemBids = y })
						.SelectMany(
							x => x.ItemBids.DefaultIfEmpty(),
							(x, y) => new {Items = x.Items, ItemBids = y});							
// 416 Records NOTE!!! This also counts the Items with No Bids. There are currently 406 bids with 10 items that have no bids as of 1/29/16		


// baseline...
//var model = allItemsBidHistory.Select(i => new { i.Items.Id, i.Items.Name, i.ItemBids});
//Console.WriteLine(model);

// http://stackoverflow.com/questions/23188855/i-get-anonymous-type-members-must-be-declared-with-a-member-assignment-when-cast
/// This is the model that will be used in the Home Index view
var model = allItemsBidHistory.Select(i => new 
		{ 	Id = i.Items.Id, 
			Name = i.Items.Name,
			Description = i.Items.Description,
			Image = i.Items.Image,
			Value = i.Items.Value,
			MinimumBid = i.Items.MinimumBid,
			//NewBid = (int?)i.ItemBids.Bid ?? 0,  // ItemListViewModel
            MyBid = (int?)i.ItemBids.Bid ?? 0,
			//HighestBid = (int?)i.Items.Bids.Max(b => b.Bid) ?? 0,  // ItemListViewModel
			HighestBid = (int?)i.Items.ItemBids.Max(b => b.Bid) ?? 0,
			UserId = (int?)i.ItemBids.UserId
			//}).OrderByDescending(i => i.MyBid);
		//}).GroupBy(i => i.Name); //.OrderByDescending(i => i.MyBid);
		}).OrderBy(i => i.Name).ThenByDescending(i => i.MyBid); //.Where(u => u.UserId == 3);

//Console.WriteLine(model.Where(u => u.UserId == 3).GroupBy(i => i.Id).Select(x => x.FirstOrDefault()));
/// This is what is currently returned by the HomeController.Index()
//Console.WriteLine(model.GroupBy(i => i.Id).Select(x => x.FirstOrDefault()).OrderBy(i => i.Name).ThenBy(i => i.Description));

// http://stackoverflow.com/questions/10637760/linq-group-by-and-select-collection
//var q2 = q1.Where(u => u.UserId == 3).GroupBy(i => new {i.ItemId, i.ItemName}).Select(g => new {g.Key, MyBid = g.ToList()});
//var modelWithMyBids = model.Where(u => u.UserId == 3).GroupBy(i => i.Id).Select(g => new {g.Key, MyBid = g.ToList()});

// http://stackoverflow.com/questions/14747680/distinct-by-one-column-and-max-from-another-column-linq
var modelWithMyBids = model.Where(u => u.UserId == 3).GroupBy(i => i.Id).Select(g => g.OrderByDescending(x => x.MyBid).FirstOrDefault());//.ThenByDescending(i => i.MyBid);
Console.WriteLine(modelWithMyBids);

var left = model.GroupBy(i => i.Id).Select(x => x.FirstOrDefault()).OrderBy(i => i.Name).ThenBy(i => i.Description);
Console.WriteLine(left);

/// Now lets show the user Bids with the winning Bids
/// As of 1/28/2016, bidsByUser has 294 records. These DO NOT include UserId = 0
var bidsByUser = UserProfiles.Join(model, u => u.UserId,
							m => m.UserId,
							(u, m) => new
								{
									UserName = u.UserName,
									UserId = u.UserId,
									ItemName = m.Name,
									ItemId = m.Id,
									Bid = m.MyBid
								});
//Console.WriteLine(bidsByUser.OrderBy(i => i.ItemName).Where(u => u.UserId == 3)); 
Console.WriteLine(bidsByUser.OrderBy(i => i.ItemName));

