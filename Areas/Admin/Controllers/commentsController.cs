using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using PagedList;

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admins")]
    public class commentsController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetComments(string term)     // get Get Comments name with AutoComplete With jQuery UI
        {
            List<string> comments = db.comments.Where(x => x.comments.ToLower().TrimStart().StartsWith(term)).Select(y => y.comments).ToList();

            return Json(comments, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/comments
        public ActionResult Index(string search, int? page)
        {
            var comments = db.comments.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                comments = comments.Where(x => x.comments.ToLower().Contains(search));
            }

            comments = comments.Include(c => c.book).Include(c => c.user);
            return View(comments.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Admin/comments/Create
        public ActionResult Create()
        {
            ViewBag.book_id = new SelectList(db.books, "book_id", "title");
            ViewBag.user_id = new SelectList(db.users, "Id", "first_name");
            return View();
        }

        // POST: Admin/comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "comment_id,comments,book_id,user_id")] comment comment)
        {
            comment.comment_data = DateTime.Now.ToString();
            if (ModelState.IsValid)
            {
                db.comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.book_id = new SelectList(db.books, "book_id", "title", comment.book_id);
            ViewBag.user_id = new SelectList(db.users, "Id", "first_name", comment.user_id);
            return View(comment);
        }

        // GET: Admin/comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.book_id = new SelectList(db.books, "book_id", "title", comment.book_id);
            ViewBag.user_id = new SelectList(db.users, "Id", "first_name", comment.user_id);
            return View(comment);
        }

        // POST: Admin/comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comment_id,comments,book_id,user_id")] comment comment)
        {
            comment.comment_data = db.comments.Find(comment.comment_id).comment_data;
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.book_id = new SelectList(db.books, "book_id", "title", comment.book_id);
            ViewBag.user_id = new SelectList(db.users, "Id", "first_name", comment.user_id);
            return View(comment);
        }


        // POST: Admin/comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comment comment = db.comments.Find(id);
            db.comments.Remove(comment);
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
