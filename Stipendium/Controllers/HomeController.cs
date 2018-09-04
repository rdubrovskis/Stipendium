using Stipendium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stipendium.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var list = db.Pageviews.ToList().OrderByDescending(i => i.ViewCount);
            var topTen = list.Take(10);

            return View(topTen);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Join()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public JsonResult GetUsers()
        {
            var result = db.Users.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult _MostPopular()
        {
            var list = db.Pageviews.ToList().OrderByDescending(i => i.ViewCount);
            var topTen = list.Take(10);

            return View(topTen);

        }

    }
}