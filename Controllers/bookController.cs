using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Library.Models;
using PagedList;

namespace Library.Controllers
{
    public class bookController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        public JsonResult GetBooks(string term)     // get books name with AutoComplete With jQuery UI
        {
            List<string> books = db.books.Where(x => x.title.ToLower().TrimStart().StartsWith(term)).Select(y => y.title).ToList();

            return Json(books, JsonRequestBehavior.AllowGet);
        }

        // GET: book
        public ActionResult Index(int? page, string searched)
        {
            if (searched != null)
            {
                return View(db.books.Where(x => x.title.ToLower().Contains(searched)).ToList().ToPagedList(page ?? 1, 8));
            }
            else
            {
                var books = db.books.Include(b => b.author).Include(b => b.publication).Include(b => b.subcategory).Include(b => b.user);
                return View(books.OrderByDescending(x=>x.book_id).ToList().ToPagedList(page ?? 1, 8));
            }
        }

        // GET: book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: book/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.author_id = new SelectList(db.authors, "author_id", "first_name");
            ViewBag.publication_id = new SelectList(db.publications, "publication_id", "name");
            ViewBag.subcategory_id = new SelectList(db.subcategories, "subcategory_id", "subcategory_name");
            ViewBag.member_id = new SelectList(db.users, "Id", "first_name");

            return View();
        }

        // POST: book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "book_id,title,edition,price,publication_id,author_id,subcategory_id,pages,description,rate,availabilty,tags,uploadedImage,uploadedFile")] book book)
        {
            if (ModelState.IsValid)
            {
                book.add_date = DateTime.Now.ToString();                                                    // Data
                var userName = User.Identity.GetUserName();
                book.member_id = db.users.Where(x=>x.UserName == userName).Select(x=>x.Id).FirstOrDefault();

                // Uplaod Photo =======================================================================================================
                string ImageName = Path.GetFileNameWithoutExtension(book.uploadedImage.FileName);    // get name
                string ImageExtension = Path.GetExtension(book.uploadedImage.FileName);                   // get extension
                ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                book.photo = "~/Uploads/Photos/books/" + ImageName;                                         // Save To DB
                ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/books/"), ImageName);             // Combine To Local Folder
                book.uploadedImage.SaveAs(ImageName);                                                       // Move it

                // Uplaod File =======================================================================================================
                string FileName = Path.GetFileNameWithoutExtension(book.uploadedFile.FileName);             // get name
                string FileExtension = Path.GetExtension(book.uploadedFile.FileName);                       // get extension
                FileName = FileName + '_' + DateTime.Now.ToString("yymmssfff") + FileExtension;                   // Not To Repeat
                book.File = "~/Uploads/Books/" + FileName;                                                  // Save To DB
                FileName = Path.Combine(Server.MapPath("~/Uploads/Books/"), FileName);                      // Combine To Local Folder
                book.uploadedFile.SaveAs(FileName);                                                         // Move it 

                db.books.Add(book);
                db.SaveChanges();
                TempData["Success"] = "Created Successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.author_id = new SelectList(db.authors, "author_id", "first_name", book.author_id);
            ViewBag.publication_id = new SelectList(db.publications, "publication_id", "name", book.publication_id);
            ViewBag.subcategory_id = new SelectList(db.subcategories, "subcategory_id", "subcategory_name", book.subcategory_id);
            ViewBag.member_id = new SelectList(db.users, "Id", "first_name", book.member_id);
            return View(book);
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
