using FCMAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FCMAuction.Controllers
{
    public class HomeController : Controller
    {
        FCMAuctionDb _db = new FCMAuctionDb();

        public ActionResult Index()
        {
            var maxBids =
                            from b in _db.ItemBids
                            group b by b.ItemId into g
                            select new { ItemId = g.Key, Bid = g.Max(b => b.Bid) };

            int bidsTotal = 0;
            int bidsCount = 0;
            if (maxBids.Any())
            {
                bidsTotal =
                                (from b in maxBids
                                 select b.Bid).Sum();
                bidsCount =
                                (from b in _db.ItemBids
                                 select b.Bid).Count();
            }

            ViewBag.BidTotal = string.Format("There are {0} bids and a total of {1:c0} in winning bids.", bidsCount, bidsTotal);

            //var model = _db.Items
            //    //.OrderByDescending(r => r.Bids.Max(bid => bid.Bid))
            //    .OrderBy(r => r.Name)
            //    .Select(r => new ItemListViewModel
            //    {
            //        Id = r.Id,
            //        Name = r.Name,
            //        Description = r.Description,
            //        Image = r.Image,
            //        Value = r.Value,
            //        MinimumBid = r.MinimumBid,
            //        NewBid = r.MinimumBid,
            //        // http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
            //        HighestBid = (int?)r.Bids.Max(b => b.Bid) ?? 0 
            //    });
            var itemBids = _db.Items.GroupJoin(_db.ItemBids,
                    i => i.Id,
                    ib => ib.ItemId,
                    (x, y) => new { Items = x, ItemBids = y })
                    .SelectMany(
                        x => x.ItemBids.DefaultIfEmpty(),
                        (x, y) => new { Items = x.Items, ItemBids = y });

            var model = itemBids.Select(i => new ItemListViewModel
            {
                Id = i.Items.Id,
                Name = i.Items.Name,
                Description = i.Items.Description,
                Image = i.Items.Image,
                Value = i.Items.Value,
                MinimumBid = i.Items.MinimumBid,
                NewBid = (int?)i.ItemBids.Bid ?? 0,
                HighestBid = (int?)i.Items.Bids.Max(b => b.Bid) ?? 0,
                UserId = (int?)i.ItemBids.UserId
            }).OrderBy(i => i.Name).ThenByDescending(i => i.NewBid);

            return View(model.GroupBy(i => i.Id).Select(x => x.FirstOrDefault()).OrderBy(i => i.Name).ThenBy(i => i.Description));
        }

        [Authorize]
        public ActionResult MyBids()
        {
            int userId = (int)Membership.GetUser().ProviderUserKey;

            var itemBids = _db.Items.GroupJoin(_db.ItemBids,
                                                    i => i.Id,
                                                    ib => ib.ItemId,
                                                    (x, y) => new { Items = x, ItemBids = y })
                                                    .SelectMany(
                                                        x => x.ItemBids.DefaultIfEmpty(),
                                                        (x, y) => new { Items = x.Items, ItemBids = y });

            var model = itemBids.Select(i => new ItemListViewModel
            {
                Id = i.Items.Id,
                Name = i.Items.Name,
                Description = i.Items.Description,
                Image = i.Items.Image,
                Value = i.Items.Value,
                MinimumBid = i.Items.MinimumBid,
                NewBid = (int?)i.ItemBids.Bid ?? 0,
                HighestBid = (int?)i.Items.Bids.Max(b => b.Bid) ?? 0,
                UserId = (int?)i.ItemBids.UserId
            }).OrderBy(i => i.Name).ThenByDescending(i => i.NewBid);

            // http://stackoverflow.com/questions/14747680/distinct-by-one-column-and-max-from-another-column-linq
            return View(model.Where(u => u.UserId == userId).GroupBy(i => i.Id).Select(g => g.OrderByDescending(x => x.NewBid).FirstOrDefault()));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (_db != null)
                _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
