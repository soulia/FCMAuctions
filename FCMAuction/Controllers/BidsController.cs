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

        public class bidTest
        {
            public int Id { get; set; }
            public int Bid { get; set; }
            public int ItemId { get; set; }
            public int UserId { get; set; }
        }

        public ActionResult BidHistory(int itemId)
        {
            int userId = (int)Membership.GetUser().ProviderUserKey;

            // test section
            //var data  = _db.Database.SqlQuery<bidTest>("Winners", DBNull.Value);

            //var allBids = from b in _db.ItemBids
            //            // join u in _db.UserProfiles
            //             //on b.UserId equals u.UserId
            //             //where b.ItemId == itemId
            //             //where b.UserId == userId
            //             orderby b.Bid descending
            //             select b.Bid;

            //foreach ( bid in )

            // end test section

            var myBids = from b in _db.ItemBids
                         join u in _db.UserProfiles
                         on b.UserId equals u.UserId
                         where b.ItemId == itemId
                         where b.UserId == userId
                         orderby b.Bid descending
                         select b.Bid;

            if (myBids.Any())
                return Content("$" + string.Join(", $",  myBids));
            else
                return Content("None");
        }

        //
        // GET: /Bids/

        [Authorize(Roles="admin")]
        public ActionResult Index([Bind(Prefix = "id")] int itemId)
        {
            //var item = _db.Items.Find(itemId);

            var allItemsBidHistory = _db.Items.GroupJoin(_db.ItemBids,
                        i => i.Id,
                        ib => ib.ItemId,
                        (x, y) => new { Items = x, ItemBids = y })
                        .SelectMany(
                            x => x.ItemBids.DefaultIfEmpty(),
                            (x, y) => new { Items = x.Items, ItemBids = y })
                            .Where(i => i.Items.Id == itemId);

            var model = allItemsBidHistory.Select(i => new
            {
                Id = i.Items.Id,
                Name = i.Items.Name,
                Description = i.Items.Description,
                Image = i.Items.Image,
                Value = i.Items.Value,
                MinimumBid = i.Items.MinimumBid,
                NewBid = (int?)i.ItemBids.Bid ?? 0,  // ItemListViewModel
                //MyBid = (int?)i.ItemBids.Bid ?? 0,
                HighestBid = (int?)i.Items.Bids.Max(b => b.Bid) ?? 0,  // ItemListViewModel
                //HighestBid = (int?)i.Items.ItemBids.Max(b => b.Bid) ?? 0,
                UserId = (int?)i.ItemBids.UserId
                //}).OrderByDescending(i => i.MyBid);
                //}).GroupBy(i => i.Name); //.OrderByDescending(i => i.MyBid);
            }).OrderBy(i => i.Id).ThenByDescending(i => i.NewBid);

            var bidsByUser = _db.UserProfiles.Join(model, u => u.UserId,
                            m => m.UserId,
                            (u, m) => new
                            {
                                UserName = u.UserName,
                                UserId = u.UserId,
                                ItemName = m.Name,
                                Description = m.Description,
                                Image = m.Image,
                                Value = m.Value,
                                MinBid = m.MinimumBid,
                                ItemId = m.Id,
                                Bid = m.NewBid
                            });

            if (bidsByUser.Count() > 0)
            {

                //var result = bidsByUser.OrderBy(i => i.ItemName).ThenBy(i => i.Description).ThenByDescending(i => i.Bid);
                var result = bidsByUser.Select(b => new BidListViewModel
                    {
                        UserId = b.UserId,
                        UserName = b.UserName,
                        ItemId = b.ItemId,
                        ItemName = b.ItemName,
                        Description = b.Description,
                        Image = b.Image,
                        Value = b.Value,
                        MinimumBid = b.MinBid,
                        Bid = b.Bid
                    }).OrderBy(b => b.ItemName).ThenBy(b => b.Description).ThenByDescending(b => b.Bid);

                if (result != null)
                    return View(result);
            }
            else
            {
                var result = model.Select(r => new BidListViewModel
                    {
                        UserId = null,
                        UserName = null,
                        ItemId = r.Id,
                        ItemName = r.Name,
                        Description = r.Description,
                        Image = r.Image,
                        Value = r.Value,
                        MinimumBid = r.MinimumBid,
                        Bid = null
                    });

                if (result != null)
                    return View(result);
            }
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
            myBid.ItemId = itemId;
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
