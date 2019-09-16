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
    [Authorize(Roles = "Admins")]
    public class publicationsController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetPublications(string term)     // get Get Publications name with AutoComplete With jQuery UI
        {
            List<string> publications = db.publications.Where(x => x.name.ToLower().TrimStart().StartsWith(term)).Select(y => y.name).ToList();

            return Json(publications, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/publications
        public ActionResult Index(string search, int? page, string sortedBy)
        {

            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortedBy) ? "Name_desc" : "";    // by default sorted will be Null from this Line

            var publications = db.publications.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                publications = publications.Where(x => x.name.ToLower().Contains(search));
            }

            switch (sortedBy)
            {
                case "Name_desc":
                    publications = publications.OrderByDescending(x => x.name);
                    break;
                default:
                    publications = publications.OrderBy(x => x.name); // acending
                    break;
            }

            return View(publications.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/publications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            publication publication = db.publications.Find(id);
            if (publication == null)
            {
                return HttpNotFound();
            }
            return View(publication);
        }

        // GET: Admin/publications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/publications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "publication_id,name,address,contact,uploadedPublicationLogo")] publication publication)
        {
            if (ModelState.IsValid)
            {
                // Uplaod Photo =======================================================================================================
                string ImageName = Path.GetFileNameWithoutExtension(publication.uploadedPublicationLogo.FileName);    // get name
                string ImageExtension = Path.GetExtension(publication.uploadedPublicationLogo.FileName);                   // get extension
                ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                publication.logo = "~/Uploads/Photos/publications/" + ImageName;                                         // Save To DB
                ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/publications/"), ImageName);             // Combine To Local Folder
                publication.uploadedPublicationLogo.SaveAs(ImageName);                                                       // Move it

                db.publications.Add(publication);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(publication);
        }

        // GET: Admin/publications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            publication publication = db.publications.Find(id);
            if (publication == null)
            {
                return HttpNotFound();
            }
            return View(publication);
        }

        // POST: Admin/publications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "publication_id,name,address,contact,uploadedPublicationLogo")] publication publication)
        {
            if (ModelState.IsValid)
            {
                if (publication.uploadedPublicationLogo != null)
                {
                    string lastPhoto = db.books.Find(publication.publication_id).photo;
                    System.IO.File.Delete(Request.MapPath(lastPhoto));  // delete From Local Folder

                    string ImageName = Path.GetFileNameWithoutExtension(publication.uploadedPublicationLogo.FileName);    // get name
                    string ImageExtension = Path.GetExtension(publication.uploadedPublicationLogo.FileName);                   // get extension
                    ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                    publication.logo = "~/Uploads/Photos/publications/" + ImageName;                                         // Save To DB
                    ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/publications/"), ImageName);             // Combine To Local Folder
                    publication.uploadedPublicationLogo.SaveAs(ImageName);                                                       // Move it
                }
                else
                {
                    publication.logo = db.publications.Find(publication.publication_id).logo;     // let the last, don't change
                }

                var local = db.Set<publication>().Local.FirstOrDefault(f => f.publication_id == publication.publication_id);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(publication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(publication);
        }


        // POST: Admin/publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            publication publication = db.publications.Find(id);
            db.publications.Remove(publication);
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
