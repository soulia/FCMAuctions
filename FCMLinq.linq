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

// http://odetocode.com/blogs/scott/archive/2007/09/24/nested-selects-in-linq-to-sql.aspx
var q = from e in Entries
          select new
          {
               EntryID = e.EntryID,
               EntryName = e.EntryName,
               EntryDescription = e.EntryDescription,
               VoterID = voterID,
               Score = (int?)(from v in e.Votes
                              where v.VoterID == voterID 
                              select v.Score).FirstOrDefault()
};


var q = from i in Items
			select new 
			{
				ItemId = i.Id,
				Name = i.Name,
				Description = i.Description,
				UserId = 3,
				Bid = (int)(from b in ItemBids
							where b.UserId == 3
							select b.Bid).FirstOrDefault()
			}
			;
			
Console.WriteLine(q);


// select my bid items and include highest bid

from b in ItemBids
	join i in Items
	on b.ItemId equals i.Id
	group b by b.ItemId into g
	//where b.UserId == 3
	//orderby b.Bid descending
	select new {i.Name, i.Description, b.Bid}


 from b in ItemBids
   join u in UserProfiles
   on b.UserId equals u.UserId
   where b.UserId == 3
   select new {b.Bid, b.ItemId}

(from u in UserProfiles
	join b in ItemBids
	on u.UserId equals b.UserId
	where u.UserId == 3
	//orderby b.ItemId
	select new {b.ItemId}).Distinct() 
	
////////////////////////////////////////	
// Max bids, totals and counts
var maxBids = 
		from b in ItemBids
		group b by b.ItemId into g
		select new { ItemId = g.Key, Bid = g.Max(b => b.Bid)};
		
int totalBids = 
		(from b in maxBids
		select b.Bid).Sum();
		
int bidCount = 
		(from b in maxBids
		select b.Bid).Count();

Console.WriteLine("Total Bids = {0:c0}", totalBids);
Console.WriteLine("Bid Count = {0}", bidCount);
Console.WriteLine(maxBids);
////////////////////////////////////////

//=============================================
// Home / Index

  //var model = Items
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

var winningBids = itemBids.GroupBy(i => i.ItemId).Select(g => new {g.Key, Bid = g.Max(b => b.Bid)});

Console.WriteLine(winningBids);

//=============================================
/// PS - Practical LINQ / More LINQ Examples
/// http://msmvps.com/blogs/deborahk/

var seq1 = Enumerable.Range(0,10);
var seq2 = Enumerable.Range(0,10).Select( i => i*i);

//Console.WriteLine(seq1);
Console.WriteLine(seq1.Intersect(seq2));
Console.WriteLine(seq1.Except(seq2));
Console.WriteLine(seq1.Concat(seq2));
Console.WriteLine(seq1.Concat(seq2).Distinct());
Console.WriteLine(seq1.Union(seq2));

var q = UserProfiles.Select(c => c.UserName);
Console.WriteLine(q);

var q = UserProfiles.Select(c => new {Name = c.UserName, Id = c.UserId});
Console.WriteLine(q);

// original working...
var itemBids = Items.Join(ItemBids, i => i.Id, 
						ib => ib.ItemId,
						(i, ib) => new 
							{
								Id = i.Id,
								Name = i.Name,
								Description = i.Description,
								//Bid = ib.Max(b => b.Bid)
								Image = i.Image,
								Value = i.Value,
								MinimumBid = i.MinimumBid,
								Bid = ib.Bid,
								UserId = ib.UserId
								//UserName = (UserProfiles.Select(u => u.UserName).Where(u => u.UserId == ib.UserId))
								//UserName = (UserProfiles.Select(new {Name = u.UserName, Id = u.UserId}).Where(x => x.Id == UserId))
							});
Console.WriteLine(itemBids);  // 379 - 288 = 91 

// now with left join...
// http://stackoverflow.com/questions/584820/how-do-you-perform-a-left-outer-join-using-linq-extension-methods
var itemBids = Items.GroupJoin(ItemBids, 
						i => i.Id, 
						ib => ib.ItemId,
						(x,y) => new 
							{ Items = x, ItemBids = y })
						.SelectMany(
							x => x.ItemBids.DefaultIfEmpty(),
							(x, y) => new {Items = x.Items, ItemBids = y});
							
//Console.WriteLine(itemBids);  // 379 - 288 = 91 

// baseline...
//var model = itemBids.Select(i => new { i.Items.Id, i.Items.Name, i.ItemBids});
//Console.WriteLine(model);

// http://stackoverflow.com/questions/23188855/i-get-anonymous-type-members-must-be-declared-with-a-member-assignment-when-cast
var model = itemBids.Select(i => new 
		{ 	Id = i.Items.Id, 
			Name = i.Items.Name,
			Description = i.Items.Description,
			Image = i.Items.Image,
			Value = i.Items.Value,
			MinimumBid = i.Items.MinimumBid,
			MyBid = (int?)i.ItemBids.Bid ?? 0,
			HighestBid = (int?)i.Items.ItemBids.Max(b => b.Bid) ?? 0,
			UserId = (int?)i.ItemBids.UserId
		//}).GroupBy(i => i.Name); //.OrderByDescending(i => i.MyBid);
		}).OrderBy(i => i.Name).ThenByDescending(i => i.MyBid); //.Where(u => u.UserId == 3);

Console.WriteLine(model.Where(u => u.UserId == 3).GroupBy(i => i.Id).Select(x => x.FirstOrDefault()));
Console.WriteLine(model.GroupBy(i => i.Id).Select(x => x.FirstOrDefault()));


//var winningBids = itemBids.GroupBy(i => new {i.ItemId, i.ItemName}).Select(g => new {g.Key.ItemId, g.Key.ItemName, Bid = g.Max(b => b.Bid)});
var winningBids = ItemBids.GroupBy(i => i.Id).Select(g => new {g.Key, Bid = (int?)g.Max(b => b.Bid) ?? 0});
Console.WriteLine(winningBids);  // 108 winning bids

var itemBidsWithWinningBids = itemBids.Join(winningBids, ib => ib.Id,
								wb => wb.Key,
								(ib, wb) => new 
									{
										Id = ib.Id,
										Name = ib.Name,
										Description = ib.Description,
										Image = ib.Image,
										Value = ib.Value,
										MinimumBid = ib.MinimumBid,
										Bid = ib.Bid,
										HighestBid = wb.Bid
									}); //.Where(u => u.UserId == 3);
Console.WriteLine(itemBidsWithWinningBids.OrderBy(i => i.Name));
								
var q1 = UserProfiles.Join(itemBids, u => u.UserId,
							ib => ib.UserId,
							(u, ib) => new
								{
									UserName = u.UserName,
									UserId = u.UserId,
									ItemName = ib.Name,
									ItemId = ib.Id,
									Bid = ib.Bid
								});
Console.WriteLine(q1.OrderBy(i => i.ItemName));  // 288
							
//Console.WriteLine(q1.Where(u => u.UserId == 3));
//Console.WriteLine(q1.Where(u => u.UserId == 3).GroupBy(i => i.ItemId));
//Console.WriteLine(q1.Where(u => u.UserId == 3).GroupBy(i => new {i.ItemId, i.ItemName}).Select(g => new {g.Key.ItemId, g.Key.ItemName, Bid = g.Max(b => b.Bid)}));

var q2 = q1.Where(u => u.UserId == 3).GroupBy(i => new {i.ItemId, i.ItemName}).Select(g => new {g.Key.ItemId, g.Key.ItemName, Bid = g.Max(b => b.Bid)});
Console.WriteLine(q2);

var q3 = q2.Join(winningBids, i => i.ItemId,
						w => w.Key,
						(i, w) => new
							{
								ItemName = i.ItemName,
								Bid = i.Bid,
								WinningBid = w.Bid
							});
Console.WriteLine(q3);


Console.WriteLine(q1.Where(u => u.UserId == 3).GroupBy(i => new {i.ItemId, i.ItemName}).Select(g => new {g.Key.ItemId, g.Key.ItemName, Bid = g.Max(b => b.Bid)}));
//Console.WriteLine(q1.Select(u => new {u.UserName, u.UserId, u.ItemId, u.Bid}).Where(i => i.UserId == 3));
Console.WriteLine(q1.Select(u => new {u.UserName, u.UserId, u.ItemId, u.Bid}).GroupBy(i => i.ItemId));
Console.WriteLine(q1.Select(u => new {u.UserName, u.UserId, u.ItemId, u.Bid}).Where(i => i.UserId == 3).Where(b => b.Bid == 2));

//=============================================

from i in Items
join b in 
(
	from b in ItemBids
	group b by b.ItemId into g
	select new { ItemId = g.Key, Bid = g.Max(b => b.Bid)}
)
 on i.Id = 
orderby i.Name 
select new 
	{ 
		Id = i.Id, 
		Name = i.Name, 
		Description = i.Description,
		Image = i.Image,
		Value = i.Value,
		NewBid = 0
		HighestBid = 42
	}


// tcp:fcm.database.windows.net,1433

from b in ItemBids
group b by new {b.ItemId, b.UserId} into g
select new { ItemId = g.Key, UserId = g.Key.UserId, winner = g.Max(b => b.Bid)}

from b in ItemBids
group b by b.ItemId into g
select new { ItemId = g.Key, winner = g.Max(b => b.Bid)}


from i in ItemBids
join w in
(
from b in ItemBids
group b by new {b.ItemId, b.UserId} into g
select new { ItemId = g.Key, UserId = g.Key.UserId, winner = g.Max(b => b.Bid)}
) 
on i equals w.ItemId
select new {ItemId = i, w.winner}


from i in ItemBids
orderby i.ItemId, i.Bid descending
select new
{
	i.Id,
	i.Bid,
	i.ItemId,
	i.UserId
}

// This works... kind of
var highest = 
	from b in ItemBids
group b by b.ItemId into g
select new { ItemId = g.Key, Bid = g.Max(b => b.Bid)};

Console.WriteLine(highest);

var winner = 
	from i in ItemBids
	join h in highest
	  on new {item = i.ItemId, bid = i.Bid} equals
	  new {item = h.ItemId, bid = h.Bid} into j 
	  orderby i.ItemId
	  select new {j};
	  
Console.WriteLine(winner);


// stored proc calls...
// C# Expression
Winners()  

// C# Statments
DataSet foo = Winners();
Console.WriteLine(foo);

var bids = 
	from b in ItemBids orderby b.Bid descending
	select b;
	
Console.WriteLine(bids);

var maxes = bids.GroupBy(x => x.ItemId, 
			(key, xs) => xs.OrderByDescending(x => x.Bid).First().Bid);
			
Console.WriteLine(maxes);

from t2 in ItemBids.GroupBy(i => i.ItemId)
	.Select(g => g.OrderByDescending(i => i.Bid).FirstOrDefault())
	
			.