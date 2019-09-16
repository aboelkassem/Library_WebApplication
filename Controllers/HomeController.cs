using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using PagedList;

namespace Library.Controllers
{   [RequireHttps]
    public class HomeController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Category(int Id)
        {
            List<subcategory> subcategories = db.subcategories.Where(x => x.category_id == Id).ToList();
            return View(subcategories);
        }
        public ActionResult user(int Id)
        {
            var user = db.users.Where(x => x.Id == Id).FirstOrDefault();
            return View(user);
        }
        public ActionResult tag(int? page,string tag)
        {
            List<book> books = db.books.Where(x => x.tags.Contains(tag)).ToList();
            return View(books.ToList().ToPagedList(page ?? 1, 8));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(comment comment)
        {
            comment.comment_data = DateTime.Now.ToString();
            if (ModelState.IsValid)
            {
                db.comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details","book",new {id = comment.book_id});
            }
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}