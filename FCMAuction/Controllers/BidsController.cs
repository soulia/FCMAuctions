using FCMAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FCMAuction.Controllers
{
    [Authorize]
    public class BidsController : Controller
    {
        FCMAuctionDb _db = new FCMAuctionDb();

        //public ActionResult BidHistory(int id)
        //{
        //    int userId = (int)Membership.GetUser().ProviderUserKey;

        //    var myBids = from  b in _db.ItemBids
        //                 join u in _db.UserProfiles
        //                 on b.UserId equals u.UserId
        //                  where b.Id == id
        //                  orderby b.Bid descending
        //                  select b;
        //    if (myBids.Any())
        //        return View(myBids);
        //    else
        //        return View();
        //}

        //
        // GET: /Bids/

        [Authorize(Roles="admin")]
        public ActionResult Index([Bind(Prefix = "id")] int itemId)
        {
            var item = _db.Items.Find(itemId);

            if(item != null)
                return View(item);
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Create(int itemId)
        {
            var allBids = from b in _db.ItemBids
                             join i in _db.Items
                             on b.ItemId equals i.Id
                             where i.Id == itemId
                             orderby b.Bid descending
                             select b;

            var minumItemBid = from i in _db.Items
                               where i.Id == itemId
                               select i;

            int highestBid = allBids.Any() ? Math.Max(allBids.First().Bid, minumItemBid.First().MinimumBid) : minumItemBid.First().MinimumBid;

            object userId = Membership.GetUser().ProviderUserKey;
            var myBid = new ItemBid();
            myBid.Bid = highestBid + 1;
            myBid.UserId = (int)userId;

            return View(myBid);
        }

        [HttpPost]
        // Obscure naming convention bug with EF - don't name parameters the same as model properties...
        // http://www.martin-brennan.com/net-mvc-4-model-binding-null-on-post/
        public ActionResult Create(ItemBid bidd)
        {
            var allBids = from b in _db.ItemBids
                          join i in _db.Items
                          on b.ItemId equals i.Id
                          where i.Id == bidd.ItemId
                          orderby b.Bid descending
                          select b;

            var minumItemBid = from i in _db.Items
                               where i.Id == bidd.ItemId
                               select i;

            int minumBid = allBids.Any() ? Math.Max(allBids.First().Bid, minumItemBid.First().MinimumBid) : minumItemBid.First().MinimumBid;

            // for this to work, make sure to set     @Html.ValidationSummary(false) in the Create.cshtml View
            if (bidd.Bid <= minumBid)
                ModelState.AddModelError("Bid", "Bid must be greater than $" + minumBid.ToString());
            else
                bidd.UserId = (int)Membership.GetUser().ProviderUserKey; 

            if(ModelState.IsValid)
            {
                _db.ItemBids.Add(bidd);
                _db.SaveChanges();
                //return RedirectToAction("Index", "Bids", new { id = bidd.ItemId });
                return RedirectToAction("Index", "Home");
            }
            return View(bidd);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
        
    }
}
