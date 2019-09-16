using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Areas.Admin.Controllers
{
    public class logsController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        // GET: Admin/logs
        public ActionResult logs()
        {
            return PartialView(db.Logs.OrderByDescending(x => x.log_it).Take(30).ToList());
        }

        //public PartialViewResult Top20()
        //{
        //    List<Log> model = db.Logs.OrderByDescending(x => x.message).Take(20).ToList();
        //    return PartialView("_Student", model);
        //}
    }
}