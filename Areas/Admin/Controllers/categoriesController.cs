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
    public class categoriesController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetCategories(string term)     // get Get Categoires name with AutoComplete With jQuery UI
        {
            List<string> categories = db.categories.Where(x => x.category_name.ToLower().TrimStart().StartsWith(term)).Select(y => y.category_name).ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/categories
        public ActionResult Index(string search, int? page, string sortedBy)
        {
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortedBy) ? "Name_desc" : "";    // by default sorted will be Null from this Line

            var categories = db.categories.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                categories = categories.Where(x => x.category_name.ToLower().Contains(search));
            }


            switch (sortedBy)
            {
                case "Name_desc":
                    categories = categories.OrderByDescending(x => x.category_name);
                    break;
                default:
                    categories = categories.OrderBy(x => x.category_name);
                    break;
            }

            return View(categories.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "category_id,category_name,description")] category category)
        {
            if (ModelState.IsValid)
            {
                db.categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "category_id,category_name,description")] category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // POST: Admin/categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            category category = db.categories.Find(id);
            db.categories.Remove(category);
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
