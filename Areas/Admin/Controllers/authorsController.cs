using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using PagedList;

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admins")]
    public class authorsController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetAuthors(string term)     // get Get Authors name with AutoComplete With jQuery UI
        {
            List<string> authors = db.authors.Where(x => x.first_name.ToLower().TrimStart().StartsWith(term)).Select(y => y.first_name).ToList();

            return Json(authors, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/authors
        public ActionResult Index(string search, int? page, string sortedBy)
        {
            ViewBag.SortFirstNameParameter = string.IsNullOrEmpty(sortedBy) ? "FirstName_desc" : "";    // by default sorted will be Null from this Line
            ViewBag.SortNationalityParameter = sortedBy == "Nationality" ? "Nationality_desc" : "Nationality";

            var authors = db.authors.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                authors = authors.Where(x => x.first_name.ToLower().Contains(search) || x.last_name.StartsWith(search));
            }

            switch (sortedBy)
            {
                case "FirstName_desc":
                    authors = authors.OrderByDescending(x => x.first_name);
                    break;
                case "Nationality_desc":
                    authors = authors.OrderByDescending(x => x.nationality);
                    break;
                case "Nationality":
                    authors = authors.OrderBy(x => x.nationality); // acending
                    break;
                default:
                    authors = authors.OrderBy(x => x.first_name); // acending
                    break;
            }

            return View(authors.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = db.authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Admin/authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "author_id,first_name,last_name,nationality,uploadedAuthorImage,description")] author author)
        {
            if (ModelState.IsValid)
            {

                // Uplaod Photo =======================================================================================================
                string ImageName = Path.GetFileNameWithoutExtension(author.uploadedAuthorImage.FileName);    // get name
                string ImageExtension = Path.GetExtension(author.uploadedAuthorImage.FileName);                   // get extension
                ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                author.photo = "~/Uploads/Photos/authors/" + ImageName;                                         // Save To DB
                ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/authors/"), ImageName);             // Combine To Local Folder
                author.uploadedAuthorImage.SaveAs(ImageName);

                db.authors.Add(author);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Admin/authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = db.authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Admin/authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "author_id,first_name,last_name,nationality,uploadedAuthorImage,description")] author author)
        {
            if (ModelState.IsValid)
            {
                if (author.uploadedAuthorImage != null)
                {
                    string lastPhoto = db.authors.Find(author.author_id).photo;
                    System.IO.File.Delete(Request.MapPath(lastPhoto));  // delete From Local Folder

                    string ImageName = Path.GetFileNameWithoutExtension(author.uploadedAuthorImage.FileName);    // get name
                    string ImageExtension = Path.GetExtension(author.uploadedAuthorImage.FileName);                   // get extension
                    ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                    author.photo = "~/Uploads/Photos/authors/" + ImageName;                                         // Save To DB
                    ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/authors/"), ImageName);             // Combine To Local Folder
                    author.uploadedAuthorImage.SaveAs(ImageName);
                }
                else
                {
                    author.photo = db.authors.Find(author.author_id).photo;     // let the last, don't change
                }

                var local = db.Set<author>().Local.FirstOrDefault(f => f.author_id == author.author_id);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // POST: Admin/authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            author author = db.authors.Find(id);
            if (System.IO.File.Exists(Request.MapPath(author.photo)))
            {
                System.IO.File.Delete(Request.MapPath(author.photo));
            }

            db.authors.Remove(author);
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
