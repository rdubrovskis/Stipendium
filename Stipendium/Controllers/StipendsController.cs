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
        private Counties counties = new Counties();

        // GET: Stipends
        public ActionResult Index(int? page)
        {
            var query = new SearchQuery
            {
                SelectedCounties = counties.List.ToArray(),
                SearchResults = db.Stipends.ToList().OrderByDescending(s=>s.Capital).ToPagedList(1, 5),
                Page = 1,
                ItemsPerPage = 5
            };

            return View("SearchResults",query);
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


        public List<Stipend> ListBuilder (string[] counties, string sTerm, string sMunicip)
        {
            List<Stipend> ResultsList = new List<Stipend>();
            if (counties != null)
            {
                foreach (var county in counties)
                {
                    var stipendsInCounty = db.Stipends.Where(s => s.County == county).ToList();
                    ResultsList.AddRange(stipendsInCounty);
                }
            }

            ResultsList = sMunicip != null ? ResultsList.Where(r => r.Municipality.Contains(sMunicip)).ToList() : ResultsList;
            ResultsList = sTerm != null ? ResultsList.Where(r => r.Title.Contains(sTerm)).ToList() : ResultsList;
            ResultsList = ResultsList.OrderByDescending(s => s.Capital).ToList();

            return (ResultsList);
        }

        public ActionResult NewPage(int page, int size, string counties, string sTerm, string sMunicip)
        {
            string[] selectedCounties = counties.Split('&');
            var query = new SearchQuery
            {
                SelectedCounties = selectedCounties,
                ItemsPerPage = size,
                SearchMunicipality = sMunicip,
                SearchTerm = sTerm,
                SearchResults = ListBuilder(selectedCounties,sTerm,sMunicip).ToPagedList(page,size)
            };

            return View("SearchResults",query);
        }

        public ActionResult SearchResults()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResults([Bind(Include = "SearchTerm,SearchMunicipality,SelectedCounties,ItemsPerPage,Page")] SearchQuery sQuery)
        {
            //sQuery.Page = sQuery.ItemsPerPage > 5 && sQuery.Page > 1 ? 1 : sQuery.Page;
            var list = ListBuilder(sQuery.SelectedCounties, sQuery.SearchTerm, sQuery.SearchMunicipality);
            var pagedList = list.ToPagedList(sQuery.Page, sQuery.ItemsPerPage);

            if(pagedList.Count() == 0)
            {
                pagedList = list.ToPagedList(1, sQuery.ItemsPerPage);
                sQuery.Page = 1;
            }

            sQuery.SearchResults = pagedList;

            return View(sQuery);
        }
    }
}
