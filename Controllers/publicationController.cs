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

namespace Library.Controllers
{
    [Authorize]
    public class publicationController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        // GET: publication
        public ActionResult Index()
        {
            return View(db.publications.ToList());
        }

        // GET: publication/Details/5
        [AllowAnonymous]
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

        // GET: publication/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: publication/Create
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
                TempData["Success"] = "Created Successfully!";
                return RedirectToAction("Index","book");
            }

            return View(publication);
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
