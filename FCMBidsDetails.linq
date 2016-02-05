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
							(x, y) => new {Items = x.Items, ItemBids = y})
							.Where(i => i.Items.Id == 3);		
							
//Console.WriteLine(allItemsBidHistory);

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
		}).OrderBy(i => i.Id).ThenByDescending(i => i.MyBid); //.Where(u => u.UserId == 3);
		
// http://stackoverflow.com/questions/14747680/distinct-by-one-column-and-max-from-another-column-linq
//var modelWithMyBids = model.GroupBy(i => i.Id).Select(g => g.OrderByDescending(x => x.MyBid).FirstOrDefault());//.ThenByDescending(i => i.MyBid);
//Console.WriteLine(modelWithMyBids);

/// Now lets show the user Bids with the winning Bids
/// As of 1/28/2016, bidsByUser has 330 records. These DO NOT include UserId = 0
var bidsByUser = UserProfiles.Join(model, u => u.UserId,
							m => m.UserId,
							(u, m) => new
								{
									UserName = u.UserName,
									UserId = u.UserId,
									Image = m.Image,
									ItemName = m.Name,
									Description = m.Description,
									Value = m.Value,
									MinBid = m.MinimumBid,
									ItemId = m.Id,
									Bid = m.MyBid
								});
//Console.WriteLine(bidsByUser.OrderBy(i => i.ItemName).Where(u => u.UserId == 3)); 
Console.WriteLine(bidsByUser.OrderBy(i => i.ItemName).ThenBy(i => i.Description).ThenByDescending(i => i.Bid));