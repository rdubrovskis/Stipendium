using LinqToExcel;
using PagedList;
using Stipendium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;

namespace Stipendium.Controllers
{
    public class StiftelsesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Counties counties = new Counties();

        // GET: Stiftelses
        public ActionResult Index()
        {
            var query = new SearchQuery
            {
                SelectedCounties = counties.List.ToArray(),
                SearchResults = db.Stiftelses.ToList().OrderByDescending(s => s.Förmögenhet).ToPagedList(1, 25),
                Page = 1,
                ItemsPerPage = 25
            };

            return View("SearchResults", query);
        }

        // GET: Stiftelses/Details/5
        [CustomAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stiftelse stiftelse = db.Stiftelses.Find(id);
            if (stiftelse == null)
            {
                return HttpNotFound();
            }

            Pageviews pViews;

            if (db.Pageviews.Where(p=>p.Stiftelse.Id == id).Count() != 0)
            {
                pViews = db.Pageviews.First(p => p.Stiftelse.Id == id);
                pViews.ViewCount++;
            }
            else
            {
                pViews = new Pageviews
                {
                    ViewCount = 1,
                    Stiftelse = db.Stiftelses.Find(id)
                };
                db.Pageviews.Add(pViews);
            }
            db.SaveChanges();

            return View(stiftelse);
        }

        public ActionResult DetailsPartial(int? id)
        {
            var stiftelse = db.Stiftelses.Find(id);
            return PartialView("Details", stiftelse);
        }

        // GET: Stiftelses/Create
        public ActionResult Create()
        {
            return PartialView();
        }

        // POST: Stiftelses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Stiftelsenr,Aktnr,Orgnr,Län,Stiftelsenamn,Kommun,Adress,CoAdress,Postnr,Postadress,Telefon,Stiftelsetyp,Status,År,Förmögenhet,Ändamål")] Stiftelse stiftelse)
        {
            if (ModelState.IsValid)
            {
                db.Stiftelses.Add(stiftelse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stiftelse);
        }

        // GET: Stiftelses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stiftelse stiftelse = db.Stiftelses.Find(id);
            if (stiftelse == null)
            {
                return HttpNotFound();
            }
            return View(stiftelse);
        }

        // POST: Stiftelses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Stiftelsenr,Aktnr,Orgnr,Län,Stiftelsenamn,Kommun,Adress,CoAdress,Postnr,Postadress,Telefon,Stiftelsetyp,Status,År,Förmögenhet,Ändamål")] Stiftelse stiftelse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stiftelse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stiftelse);
        }

        // GET: Stiftelses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stiftelse stiftelse = db.Stiftelses.Find(id);
            if (stiftelse == null)
            {
                return HttpNotFound();
            }
            return View(stiftelse);
        }

        // POST: Stiftelses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stiftelse stiftelse = db.Stiftelses.Find(id);
            db.Stiftelses.Remove(stiftelse);
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

        [Authorize(Roles = "Admin")]
        public ActionResult UploadExcel()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UploadExcel(Stiftelse stiftelse, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.FileName.EndsWith(".xls") || FileUpload.FileName.EndsWith(".xlsx"))
                {


                    string filename = Path.GetFileName(FileUpload.FileName);
                    string targetpath = Server.MapPath("~\\App_Data\\");
                    if (!System.IO.Directory.Exists(targetpath))
                    {
                        System.IO.Directory.CreateDirectory(targetpath);
                    }
                    string pathToExcelFile = Path.Combine(targetpath + filename);
                    FileUpload.SaveAs(pathToExcelFile);
                    var file = new FileInfo(pathToExcelFile);

                    int succeeded = 0;
                    int failed = 0;

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    using (var stream = System.IO.File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read))
                    {

                        // Auto-detect format, supports:
                        //  - Binary Excel files (2.0-2003 format; *.xls)
                        //  - OpenXml Excel files (2007 format; *.xlsx)
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {

                            // 2. Use the AsDataSet extension method
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

                            // The result of each spreadsheet is in result.Tables
                            foreach (DataTable table in result.Tables)
                            {
                                foreach (DataRow row in table.Rows)
                                {
                                    string stiftNr = row["Stiftelsenr"].ToString();

                                    if (row["Stiftelsenr"].ToString() != "" && row["Aktnr"].ToString() != "" && row["Stiftelsenamn"].ToString() != "" && db.Stiftelses.Where(s => s.Stiftelsenr == stiftNr).Count() == 0)
                                    {
                                        var stift = new Stiftelse
                                        {
                                            Stiftelsenr = stiftNr,
                                            Aktnr = row["Aktnr"].ToString(),
                                            Orgnr = row["Org.nr"].ToString(),
                                            Län = row["Län"].ToString(),
                                            Stiftelsenamn = row["Stiftelsenamn"].ToString(),
                                            Kommun = row["Kommun"].ToString(),
                                            Adress = row["Adress"].ToString(),
                                            CoAdress = row["C/o adress"].ToString(),
                                            Postnr = row["Postnr"].ToString(),
                                            Postadress = row["Postadress"].ToString(),
                                            Telefon = row["Telefon"].ToString(),
                                            Stiftelsetyp = row["Stiftelsetyp"].ToString(),
                                            Status = row["Status"].ToString(),
                                            År = row["År"].ToString(),
                                            Förmögenhet = row["Förmögenhet"].ToString(),
                                            Ändamål = row["Ändamål"].ToString(),
                                            DateCreated = DateTime.Now,
                                            LastModified = DateTime.Now

                                        };
                                        succeeded++;
                                        db.Stiftelses.Add(stift);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        failed++;
                                    }
                                }
                            }
                        }

                        
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    //return Json("success", JsonRequestBehavior.AllowGet);
                    //return Json(new { upload = "success", s = succeeded, f = failed },JsonRequestBehavior.AllowGet);
                    ViewBag.Successes = succeeded;
                    ViewBag.Failures = failed;
                    return View();
                }
                else
                {
                    ViewBag.ErrorMsg = "Only .xls and .xlsx file formats allowed";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMsg = "No file selected";
                return View();
            }
        }

        public List<Stiftelse> ListBuilder(string[] counties, string sTerm, string sMunicip)
        {
            if(!string.IsNullOrWhiteSpace(sTerm))
            {
                AddSearchTerm(sTerm);
            }

            List<Stiftelse> ResultsList = new List<Stiftelse>();
            if (counties != null)
            {
                foreach (var county in counties)
                {
                    var stipendsInCounty = db.Stiftelses.Where(s => s.Län == county).ToList();
                    ResultsList.AddRange(stipendsInCounty);
                }
            }

            ResultsList = sMunicip != null ? ResultsList.Where(r => r.Kommun.Contains(sMunicip)).ToList() : ResultsList;
            ResultsList = sTerm != null ? ResultsList.Where(r => r.Stiftelsenamn.Contains(sTerm) || r.Ändamål.Contains(sTerm)).ToList() : ResultsList;
            ResultsList = ResultsList.OrderByDescending(s => s.Förmögenhet).ToList();

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
                SearchResults = ListBuilder(selectedCounties, sTerm, sMunicip).ToPagedList(page, size)
            };

            return View("SearchResults", query);
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
            IPagedList<Stiftelse> pagedList = list.ToPagedList(sQuery.Page, sQuery.ItemsPerPage);

            if (pagedList.Count() == 0)
            {
                pagedList = list.ToPagedList(1, sQuery.ItemsPerPage);
                sQuery.Page = 1;
            }

            sQuery.SearchResults = pagedList;

            return View(sQuery);
        }

        public void AddSearchTerm (string term)
        {
            var exists = db.SearchTerms.Where(s => s.Term == term).FirstOrDefault();
            if(exists == null)
            {
                db.SearchTerms.Add(new SearchTerm {Term = term, LastSearched = DateTime.Now, TimesSearched = 1 });
                db.SaveChanges();
            }
            else
            {
                exists.LastSearched = DateTime.Now;
                exists.TimesSearched++;
                db.SaveChanges();
            }
        }

    }
}
