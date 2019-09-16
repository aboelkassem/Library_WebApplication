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
    public class subcategoriesController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetSubCategories(string term)     // get Sub Categories name with AutoComplete With jQuery UI
        {
            List<string> subcategories = db.subcategories.Where(x => x.subcategory_name.ToLower().TrimStart().StartsWith(term)).Select(y => y.subcategory_name).ToList();

            return Json(subcategories, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/subcategories
        public ActionResult Index(string search, int? page, string sortedBy)
        {
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortedBy) ? "Name_desc" : "";    // by default sorted will be Null from this Line

            var subcategories = db.subcategories.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                subcategories = subcategories.Where(x => x.subcategory_name.ToLower().Contains(search));
            }

            switch (sortedBy)
            {
                case "Name_desc":
                    subcategories = subcategories.OrderByDescending(x => x.subcategory_name);
                    break;
                default:
                    subcategories = subcategories.OrderBy(x => x.subcategory_name);
                    break;
            }

            subcategories = subcategories.Include(s => s.category);
            return View(subcategories.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/subcategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subcategory subcategory = db.subcategories.Find(id);
            if (subcategory == null)
            {
                return HttpNotFound();
            }
            return View(subcategory);
        }

        // GET: Admin/subcategories/Create
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }

        // POST: Admin/subcategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subcategory_id,subcategory_name,category_id")] subcategory subcategory)
        {
            if (ModelState.IsValid)
            {
                db.subcategories.Add(subcategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", subcategory.category_id);
            return View(subcategory);
        }

        // GET: Admin/subcategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subcategory subcategory = db.subcategories.Find(id);
            if (subcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", subcategory.category_id);
            return View(subcategory);
        }

        // POST: Admin/subcategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "subcategory_id,subcategory_name,category_id")] subcategory subcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", subcategory.category_id);
            return View(subcategory);
        }


        // POST: Admin/subcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            subcategory subcategory = db.subcategories.Find(id);
            db.subcategories.Remove(subcategory);
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
