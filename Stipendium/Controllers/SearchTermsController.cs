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
    public class SearchTermsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SearchTerms
        public ActionResult Index()
        {
            return View(db.SearchTerms.ToList());
        }

        // GET: SearchTerms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchTerm searchTerm = db.SearchTerms.Find(id);
            if (searchTerm == null)
            {
                return HttpNotFound();
            }
            return View(searchTerm);
        }

        // POST: SearchTerms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SearchTerm searchTerm = db.SearchTerms.Find(id);
            db.SearchTerms.Remove(searchTerm);
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

        public JsonResult PopularSearches()
        {
            var results = db.SearchTerms.ToList().OrderByDescending(s => s.TimesSearched).Take(10).ToArray();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}
