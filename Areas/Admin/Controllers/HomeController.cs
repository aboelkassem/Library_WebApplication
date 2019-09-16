using Library.Models;
using Library.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admins")]
    public class HomeController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        // GET: Admin/Home
        public ActionResult Index()
        {

            AllTables MyModel = new AllTables();
            MyModel.books = db.books.ToList();
            MyModel.users = db.users.ToList();
            MyModel.authors = db.authors.ToList();
            MyModel.categories = db.categories.ToList();
            MyModel.subcategories = db.subcategories.ToList();
            MyModel.comments = db.comments.ToList();
            MyModel.publications = db.publications.ToList();
            MyModel.logs = db.Logs.ToList();

            return View(MyModel);
        }
    }
}