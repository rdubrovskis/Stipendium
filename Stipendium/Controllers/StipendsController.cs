using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Stipendium.Models;

namespace Stipendium.Controllers
{
    public class StipendsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stipends
        public ActionResult Index()
        {
            return View(db.Stipends.ToList());
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
    }
}
