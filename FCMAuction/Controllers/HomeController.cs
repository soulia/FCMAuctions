using FCMAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            int bidsTotal =
                (from b in maxBids
                 select b.Bid).Sum();

            ViewBag.BidTotal = string.Format("Auction Bid Total: {0:c0}", bidsTotal);

            var model = _db.Items
                //.OrderByDescending(r => r.Bids.Max(bid => bid.Bid))
                .OrderBy(r => r.Name)
                .Select(r => new ItemListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Image = r.Image,
                    Value = r.Value,
                    MinimumBid = r.MinimumBid,
                    NewBid = r.MinimumBid,
                    // http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
                    HighestBid = (int?)r.Bids.Max(b => b.Bid) ?? 0 
                });
                
            return View(model);
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
