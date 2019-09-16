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
    public class authorController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        // GET: author
        public ActionResult Index()
        {
            return View(db.authors.ToList());
        }
        [AllowAnonymous]
        // GET: author/Details/5
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

        // GET: author/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: author/Create
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
                TempData["Success"] = "Created Successfully!";
                return RedirectToAction("Index", "book");
            }

            return View(author);
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
