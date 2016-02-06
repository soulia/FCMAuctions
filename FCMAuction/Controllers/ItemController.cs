using FCMAuction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FCMAuction.Controllers
{
    public class ItemController : Controller
    {
        FCMAuctionDb _db = new FCMAuctionDb();

        //
        // GET: /Item/

        public ActionResult Index()
        {
            return View(_db.Items.ToList());
        }
        //        //
        //        // GET: /Items/Bid/5

        //        public ActionResult Bid(int id)
        //        {
        //            var bid = _bids.Single(b => b.Id == id);
        //            return View(bid);
        //        }

        //        //
        //        // POST: /Items/Bid/5

        //        [HttpPost]
        //        public ActionResult Bid(int id, FormCollection collection)
        //        {
        //            var bid = _bids.Single(b => b.Id == id);
        //            if (TryUpdateModel(bid))
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            return View(bid);
        //        }
        //

        // GET: /Items/Bid/5

        public ActionResult Bid(int id)
        {
           // var bid = _db.Items.Single(b => b.Id == id);
           // return View(bid);
            Item item = _db.Items.Find(id);
            if (item == null)
                return HttpNotFound();
            return View(item);
        }

        //
        // POST: /Items/Bid/5

        [HttpPost]
        public ActionResult Bid(Item item)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(item);
        }

        //// GET: /Item/Details/5

        public ActionResult Details(int id = 0)
        {
            Item item = _db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
            //var model = _db.Items
            //    .Where(r => r.Id == id)
            //    //.OrderByDescending(r => r.Bids.Max(bid => bid.Bid))
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
            //return View(model);
        }

        //
        // GET: /Item/Create
        //[Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Item/Create

        [HttpPost]
        //[Authorize(Roles="admin")]
        public ActionResult Create(Item item)
        {
            if(ModelState.IsValid)
            {
                _db.Items.Add(item);
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(item);
        }

        //
        // GET: /Item/Edit/5
         [Authorize(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            Item item = _db.Items.Find(id);
            if (item == null)
                return HttpNotFound();
            return View(item);
        }

        //
        // POST: /Item/Edit/5

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Item item)
        {
            if(ModelState.IsValid)
            {
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(item);
        }

        //
        // GET: /Item/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Item item = _db.Items.Find(id);
            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        //
        // POST: /Item/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = _db.Items.Find(id);
            _db.Items.Remove(item);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if(_db != null)
                 _db.Dispose();
            base.Dispose(disposing);
        }
        
    }
}
