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
            //var model = _db.Items.ToList();
            //var model = from i in _db.Items
            //            orderby i.Name
            //            select i;
            var model = _db.Items
                .OrderByDescending(r => r.Bids.Max(bid => bid.Bid))
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

        // new { id = item.Id, bid = item.NewBid }) |
        //public ActionResult BidMe(int id, int bid)
        //{
        //    var newBid = new ItemBid();
        //    newBid.ItemId = id;
        //    newBid.Bid = bid + 2;

        //    if (ModelState.IsValid)
        //    {
        //        _db.ItemBids.Add(newBid);
        //        _db.SaveChanges();
        //        //return RedirectToAction("Index");
        //    }

        //    return RedirectToAction("Index");
        //}

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
