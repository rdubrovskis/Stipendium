using PagedList;
using Stipendium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Stipendium.Controllers
{
    public class StipendsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stipends
        public ActionResult Index(int? page)
        {

            var list = db.Stipends.ToList();
            int pageSize = 5;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


            return View(list.ToPagedList(pageIndex, pageSize));
        }

        // GET: Stipends/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stipend stipend = db.Stipends.Find(id);
            if (stipend == null)
            {
                return HttpNotFound();
            }
            return View(stipend);
        }

        // GET: Stipends/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stipends/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,AddressLine1,AddressLine2,PostNr,County,ContactInfo,OrgNr,Description,Capital,AcceptsApplications")] Stipend stipend)
        {
            if (ModelState.IsValid)
            {
                stipend.ID = Guid.NewGuid().ToString();
                db.Stipends.Add(stipend);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stipend);
        }

        // GET: Stipends/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stipend stipend = db.Stipends.Find(id);
            if (stipend == null)
            {
                return HttpNotFound();
            }
            return View(stipend);
        }

        // POST: Stipends/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,AddressLine1,AddressLine2,PostNr,County,ContactInfo,OrgNr,Description,Capital,AcceptsApplications")] Stipend stipend)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stipend).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stipend);
        }

        // GET: Stipends/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stipend stipend = db.Stipends.Find(id);
            if (stipend == null)
            {
                return HttpNotFound();
            }
            return View(stipend);
        }

        // POST: Stipends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Stipend stipend = db.Stipends.Find(id);
            db.Stipends.Remove(stipend);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult _Search()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Search([Bind(Include = "SearchTerm,SelectedCounties,ItemsPerPage")] SearchQuery sQuery)
        {

            var stipList = new List<Stipend>();
            //stipList = string.IsNullOrWhiteSpace(sQuery.SearchTerm) ? db.Stipends.ToList() : db.Stipends.Where(s => s.Title.Contains(sQuery.SearchTerm)).ToList();

            if (sQuery.SelectedCounties != null)
            {
                foreach (var county in sQuery.SelectedCounties)
                {
                    var stipendsInCounty = db.Stipends.Where(s => s.County == county).ToList();
                    stipList.AddRange(stipendsInCounty);
                }
            }

           
            stipList = sQuery.SearchTerm != null ? stipList.Where(s => s.Title.Contains(sQuery.SearchTerm)).ToList() : stipList;

            ViewData["Query"] = sQuery;

            return View("Index", stipList.ToPagedList(1, sQuery.ItemsPerPage));
        }

    }
}
