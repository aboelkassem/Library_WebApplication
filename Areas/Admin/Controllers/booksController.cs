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
    public class booksController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        public JsonResult GetBooks(string term)     // get books name with AutoComplete With jQuery UI
        {
            List<string> books = db.books.Where(x => x.title.ToLower().TrimStart().StartsWith(term)).Select(y => y.title).ToList();

            return Json(books, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/books
        public ActionResult Index(string search , int? page, string sortedBy)
        {

            ViewBag.SortTitleParameter = string.IsNullOrEmpty(sortedBy) ? "Title_desc" : "";    // by default sorted will be Null from this Line
            ViewBag.SortPriceParameter = sortedBy == "Price" ? "Price_desc" : "Price";
            ViewBag.SortPagesNumberParameter = sortedBy == "Pages_Number" ? "Pages_Number_desc" : "Pages_Number";
            ViewBag.SortAddedDateParameter = sortedBy == "Added_Date" ? "Added_Date_desc" : "Added_Date";
            ViewBag.SortRateParameter = sortedBy == "Rate" ? "Rate_desc" : "Rate";

            var books = db.books.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                books = books.Where(x => x.title.ToLower().Contains(search));
            }

            switch (sortedBy)
            {
                case "Title_desc":
                    books = books.OrderByDescending(x => x.title);
                    break;
                case "Price_desc":
                    books = books.OrderByDescending(x => x.price);
                    break;
                case "Price":
                    books = books.OrderBy(x => x.price); // acending
                    break;
                case "Pages_Number_desc":
                    books = books.OrderByDescending(x => x.pages);
                    break;
                case "Pages_Number":
                    books = books.OrderBy(x => x.pages); // acending
                    break;
                case "Added_Date_desc":
                    books = books.OrderByDescending(x => x.add_date);
                    break;
                case "Added_Date":
                    books = books.OrderBy(x => x.add_date); // acending
                    break;
                case "Rate_desc":
                    books = books.OrderByDescending(x => x.rate);
                    break;
                case "Rate":
                    books = books.OrderBy(x => x.rate); // acending
                    break;
                default:
                    books = books.OrderBy(x => x.title); // acending
                    break;
            }

            books = books.Include(b => b.author).Include(b => b.publication).Include(b => b.subcategory).Include(b => b.user);
            return View(books.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/books/Details/5
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

        // GET: Admin/books/Create
        public ActionResult Create()
        {
            ViewBag.author_id = new SelectList(db.authors, "author_id", "first_name");
            ViewBag.publication_id = new SelectList(db.publications, "publication_id", "name");
            ViewBag.subcategory_id = new SelectList(db.subcategories, "subcategory_id", "subcategory_name");
            ViewBag.member_id = new SelectList(db.users, "Id", "first_name");
            return View();
        }

        // POST: Admin/books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "book_id,title,edition,price,publication_id,author_id,subcategory_id,pages,description,rate,availabilty,member_id,tags,uploadedImage,uploadedFile")] book book)
        {
            if (ModelState.IsValid)
            {
                book.add_date = DateTime.Now.ToString();                                                    // Data

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

        // GET: Admin/books/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.author_id = new SelectList(db.authors, "author_id", "first_name", book.author_id);
            ViewBag.publication_id = new SelectList(db.publications, "publication_id", "name", book.publication_id);
            ViewBag.subcategory_id = new SelectList(db.subcategories, "subcategory_id", "subcategory_name", book.subcategory_id);
            ViewBag.member_id = new SelectList(db.users, "Id", "first_name", book.member_id);
            return View(book);
        }

        // POST: Admin/books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "book_id,title,edition,price,publication_id,author_id,subcategory_id,pages,description,add_date,rate,availabilty,member_id,tags,uploadedImage,uploadedFile")] book book)
        {
            if (ModelState.IsValid)
            {
                // Uplaod Photo =======================================================================================================

                if (book.uploadedImage != null)
                {
                    string lastPhoto = db.books.Find(book.book_id).photo;
                    System.IO.File.Delete(Request.MapPath(lastPhoto));  // delete From Local Folder

                    string ImageName = Path.GetFileNameWithoutExtension(book.uploadedImage.FileName);           // get name
                    string ImageExtension = Path.GetExtension(book.uploadedImage.FileName);                     // get extension
                    ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                    book.photo = "~/Uploads/Photos/books/" + ImageName;                                         // Save To DB
                    ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/books/"), ImageName);             // Combine To Local Folder
                    book.uploadedImage.SaveAs(ImageName);                                                       // Move it
                }
                else
                {
                    book.photo = db.books.Find(book.book_id).photo;     // let the last, don't change
                }

                // Uplaod File =======================================================================================================

                if (book.uploadedFile != null)
                {
                    string lastFile = db.books.Find(book.book_id).File;
                    System.IO.File.Delete(Request.MapPath(lastFile));          // delete From Local Folder

                    string FileName = Path.GetFileNameWithoutExtension(book.uploadedFile.FileName);             // get name
                    string FileExtension = Path.GetExtension(book.uploadedFile.FileName);                       // get extension
                    FileName = FileName + '_' + DateTime.Now.ToString("yymmssfff") + FileExtension;                   // Not To Repeat
                    book.File = "~/Uploads/Books/" + FileName;                                                  // Save To DB
                    FileName = Path.Combine(Server.MapPath("~/Uploads/Books/"), FileName);                      // Combine To Local Folder
                    book.uploadedFile.SaveAs(FileName);                                                         // Move it 
                }
                else
                {
                    book.File = db.books.Find(book.book_id).File;   // let the last, don't change
                }


                var local = db.Set<book>().Local.FirstOrDefault(f => f.book_id == book.book_id);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Changed Successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.author_id = new SelectList(db.authors, "author_id", "first_name", book.author_id);
            ViewBag.publication_id = new SelectList(db.publications, "publication_id", "name", book.publication_id);
            ViewBag.subcategory_id = new SelectList(db.subcategories, "subcategory_id", "subcategory_name", book.subcategory_id);
            ViewBag.member_id = new SelectList(db.users, "Id", "first_name", book.member_id);
            return View(book);
        }

        // POST: Admin/books/Delete/5
        [HttpPost] 
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            book book = db.books.Find(id);
            if (System.IO.File.Exists(Request.MapPath(book.File)))
            {
                System.IO.File.Delete(Request.MapPath(book.File));
            }
            if (System.IO.File.Exists(Request.MapPath(book.photo)))
            {
                System.IO.File.Delete(Request.MapPath(book.photo));
            }
            db.books.Remove(book);
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
