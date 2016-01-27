SELECT u.UserName, b.Id as BidId, i.Id as ItemId, i.Name, i.Description, b.Bid
  FROM UserProfile as u
  right outer JOIN ItemBids as b  -- for all bids including UserId = 0
    ON u.UserId = b.UserId
    --where u.UserId = 3
  JOIN Items as i
	ON b.ItemId = i.id
	ORDER BY u.UserName


GO

select * from UserProfile
select * from Items


select * from ItemBids order by ItemId, Bid desc

select UserId, MAX(Bid) AS MaxBidId
from ItemBids
group by UserId


-- http://stackoverflow.com/questions/612231/how-can-i-select-rows-with-maxcolumn-value-distinct-by-another-column-in-sql
-- http://stackoverflow.com/questions/5015279/how-to-get-the-sum-of-all-column-values-in-the-last-row-of-a-resultset
-- http://stackoverflow.com/questions/1925176/sql-server-2008-top-10-and-distinct-together
-- winning bids

select i.Name, w.ItemId, i.Description, i.Value, w.Bid, w.UserId, u.UserName from Items as i
join
(
select winners.*
from ItemBids AS winners
join
(
select ItemId, MAX(Bid) AS MaxBidId
from ItemBids
group by ItemId
) AS groupedMaxBids
on winners.ItemId = groupedMaxBids.ItemId
and winners.Bid = groupedMaxBids.MaxBidId
--order by UserId, winners.ItemId 
) as w
on i.Id = w.ItemId
left outer join UserProfile as u
on u.UserId = w.UserId
--order by UserId
order by Name

--where w.UserId = 3
order by w.UserId, w.ItemId 
--order by w.ItemId asc





/*	LINQ  */

from p in Products
let spanishOrders = p.OrderDetails.Where (o => o.Order.ShipCountry == "Spain")
where spanishOrders.Any()
orderby p.ProductName
select new
{
	p.ProductName,
	p.Category.CategoryName,
	Orders = spanishOrders.Count(),	
	TotalValue = spanishOrders.Sum (o => o.UnitPrice * o.Quantity)
}





