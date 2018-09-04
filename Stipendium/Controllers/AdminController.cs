using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Stipendium.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Stipendium.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult EditAdmin(string idNr)
        {
            var user = db.Users.Find(idNr);
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin([Bind(Include = "Id,Email,UserName,PhoneNumber")] ApplicationUser model)
        {
            var user = db.Users.Find(model.Id);
            if (ModelState.IsValid)
            {
                user.Email = model.Email;
                user.UserName = user.Email;
                user.PhoneNumber = model.PhoneNumber;
                
                db.SaveChanges();
            }
            return RedirectToAction("UserAccounts");

        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);

            if (user.Roles.Count > 0)
            {
                if (user.GetType() == typeof(CompanyUser))
                {
                    var cUser = db.Users.OfType<CompanyUser>().Single(u => u.Id == user.Id);
                    db.Stiftelses.Remove(cUser.Stiftelse);
                }
            }

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult UserAccounts()
        {
            var list = db.Users.ToList();
            return View(list);
        }
        public JsonResult GetPopularStiftelses()
        {
            var list = db.Pageviews.ToList().OrderByDescending(i => i.ViewCount);
            var result = list.Take(10);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIdleUsers()
        {
            var list = db.Users.ToList();
            DateTimeOffset cutoff = DateTime.Now.AddDays(-30);
            list = db.Users.Where(u => u.LastActivityDate.CompareTo(cutoff) < 0).ToList();


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteNonTrending()
        {
            SqlCommand cmd = new SqlCommand("OutdatedSearches", new SqlConnection());
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.ConnectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=aspnet-Stipendium-20180821113020;Integrated Security=True";
            cmd.Connection.Open();
            cmd.ExecuteReader();
            return Json("succ", JsonRequestBehavior.AllowGet);
        }

    }
}
