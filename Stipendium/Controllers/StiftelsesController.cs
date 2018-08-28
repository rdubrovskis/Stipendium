using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Stipendium.Models;
using LinqToExcel;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using PagedList;

namespace Stipendium.Controllers
{
    public class StiftelsesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Counties counties = new Counties();

        // GET: Stiftelses
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var query = new SearchQuery
            {
                SelectedCounties = counties.List.ToArray(),
                SearchResults = db.Stiftelses.ToList().OrderByDescending(s=>s.Förmögenhet).ToPagedList(1, 25),
                Page = 1,
                ItemsPerPage = 25
            };

            return View("SearchResults", query);
        }

        // GET: Stiftelses/Details/5
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
            return View(stiftelse);
        }

        // GET: Stiftelses/Create
        public ActionResult Create()
        {
            return View();
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


                    string filename = DateTime.Now.ToShortDateString() + "_" + FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    OleDbDataAdapter adapter;
                    var ds = new DataSet();

                    //string sheetName = "Norrbotten";

                        int succeeded = 0;
                        int failed = 0;

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    //var firstRow = excelFile.WorksheetNoHeader().First();
                    foreach (var sheet in excelFile.GetWorksheetNames())
                    {
                        string query = string.Format("SELECT * FROM [{0}$]", sheet);
                        adapter = new OleDbDataAdapter(query, connectionString);
                        adapter.Fill(ds, sheet);

                        DataTable dtable = ds.Tables[sheet];
                        var region = from a in excelFile.Worksheet(sheet) select a;
                        
                        List<string> errors = new List<string>();

                        foreach (var a in region)
                        {
                            try
                            {
                                if (a["Stiftelsenr"] != "" && a["Aktnr"] != "" && a["Stiftelsenamn"] != "")
                                {
                                    Stiftelse TU = new Stiftelse
                                    {
                                        Stiftelsenr = a["Stiftelsenr"],
                                        Aktnr = a["Aktnr"],
                                        Orgnr = a["Org#nr"],
                                        Län = a["Län"],
                                        Stiftelsenamn = a["Stiftelsenamn"],
                                        Kommun = a["Kommun"],
                                        Adress = a["Adress"],
                                        CoAdress = a["C/o adress"],
                                        Postnr = a["Postnr"],
                                        Postadress = a["Postadress"],
                                        Telefon = a["Telefon"],
                                        Stiftelsetyp = a["Stiftelsetyp"],
                                        Status = a["Status"],
                                        År = a["År"].ToString(),
                                        Förmögenhet = a["Förmögenhet"],
                                        Ändamål = a["Ändamål"]

                                    };

                                    TU.Förmögenhet = TU.Förmögenhet == "" ? "0" : TU.Förmögenhet;
                                    db.Stiftelses.Add(TU);

                                    db.SaveChanges();

                                    succeeded++;

                                }
                                else
                                {
                                    //data.Add("<ul>");
                                    //if (a["Stiftelsenr"] == "" || a["Stiftelsenr"] == null) errors.Add("Stiftelsenr missing on row " + a.);
                                    //if (a["Aktnr"] == "" || a["Aktnr"] == null) data.Add("<li> Address is required</li>");
                                    //if (a["Org#nr"] == "" || a["Org#nr"] == null) data.Add("<li>ContactNo is required</li>");

                                    //data.Add("</ul>");
                                    //data.ToArray();
                                    //return Json(data, JsonRequestBehavior.AllowGet);
                                    failed++;
                                }
                            }

                            catch (DbEntityValidationException ex)
                            {
                                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                                {

                                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                                    {

                                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

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
    }
}
