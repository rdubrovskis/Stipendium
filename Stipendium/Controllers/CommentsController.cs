using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Stipendium.Models;
using PagedList;

namespace Stipendium.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index(int stiftID)
        {
            return PartialView(db.Comments.Where(c=>c.Stiftelse.Id == stiftID).OrderByDescending(i=>i.CommentDate).ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdminRole = db.Roles.FirstOrDefault(r => r.Name == "Admin").Id;
            return PartialView(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int stiftID, string userID)
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CurrentUser = currentUser.FirstName + " " + currentUser.LastName.First();
            var comment = new Comment
            {
                Stiftelse = db.Stiftelses.Find(stiftID),
                CommentDate = DateTime.Now
            };
            return PartialView(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Body,CommentDate")] Comment comment, int stiftID)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        comment.Stiftelse = db.Stiftelses.Find(stiftID);
        //        comment.Commenter = db.Users.Find(User.Identity.GetUserId());
        //        db.Comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Details", "Stiftelses", new { id = comment.Stiftelse.Id });
        //    }

        //    return View(comment);
        //}

        public ActionResult PostComment(int id, string body)
        {
            var comment = new Comment
            {
                Stiftelse = db.Stiftelses.Find(id),
                Commenter = db.Users.Find(User.Identity.GetUserId()),
                CommentDate = DateTime.Now,
                Body = body
            };
            db.Comments.Add(comment);
            db.SaveChanges();
            return new JsonResult { Data = "success", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,CommentDate")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            int stiftId = comment.Stiftelse.Id;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details","Stiftelses",new { id = stiftId });
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
