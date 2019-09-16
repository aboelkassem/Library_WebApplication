using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Library.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admins")]
    public class usersController : Controller
    {
        private ApplicationUserManager _userManager;
        private LibraryDBContext db = new LibraryDBContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public JsonResult IsUerNameAvailable(user model,string username)   // check if the user is exist or not in field without going to Server .... the parameters must be like name in model
        {

            var u = db.users.Where(a => a.Id == model.Id && a.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (u == null)
            {
                db.users.Add(new user { UserName = username });
                db.SaveChanges();
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json("Email already exists", JsonRequestBehavior.AllowGet);
            }
            //return Json(!db.users.Any(user => user.UserName == username), JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsPhoneAvailable(string phone)   // check if the user phone is exist or not in field without going to Server
        {
            return Json(!db.users.Any(user => user.PhoneNumber == phone), JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsEmailAvailable(string email)   // check if the user email is exist or not in field without going to Server
        {
            return Json(!db.users.Any(user => user.Email == email), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetUsers(string term)     // get Users name with AutoComplete With jQuery UI
        {
            List<string> users = db.users.Where(x => x.UserName.ToLower().TrimStart().StartsWith(term)).Select(y => y.UserName).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/users
        public ActionResult Index(string search , int? page, string sortedBy)
        {
            ViewBag.SortFirstNameParameter = string.IsNullOrEmpty(sortedBy) ? "FirstName_desc" : "";    // by default sorted will be Null from this Line
            ViewBag.SortLastNameParameter = sortedBy == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.SortJoinedDateParameter = sortedBy == "JoinedDate" ? "JoinedDate_desc" : "JoinedDate";
            ViewBag.SortUsernameParameter = sortedBy == "Username" ? "Username_desc" : "Username";

            var users = db.users.AsQueryable(); // to allow filter in data (Where(....)) and store it in Var

            if (search != null)
            {
                users = users.Where(x => x.UserName.ToLower().Contains(search));
            }

            switch (sortedBy)
            {
                case "FirstName_desc":
                    users = users.OrderByDescending(x => x.first_name);
                    break;
                case "LastName_desc":
                    users = users.OrderByDescending(x => x.last_name);
                    break;
                case "LastName":
                    users = users.OrderBy(x => x.last_name); // acending
                    break;
                case "JoinedDate_desc":
                    users = users.OrderByDescending(x => x.join_data);
                    break;
                case "JoinedDate":
                    users = users.OrderBy(x => x.join_data); // acending
                    break;
                case "Username_desc":
                    users = users.OrderByDescending(x => x.UserName);
                    break;
                case "Username":
                    users = users.OrderBy(x => x.UserName); // acending
                    break;
                default:
                    users = users.OrderBy(x => x.first_name); // acending
                    break;
            }

            return View(users.ToList().ToPagedList(page ?? 1, 8));
        }

        // GET: Admin/users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,first_name,last_name,Email,PasswordHash,gender,UserName,PhoneNumber,EmailConfirmed,PhoneNumberConfirmed,uploadedUserImage")] user user)
        {
            if (db.users.Any(x => x.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "UserName already in use, Try Different One");
            }
            if (db.users.Any(x => x.PhoneNumber == user.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already in use, Try Different One");
            }
            if (db.users.Any(x => x.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use, Try Different One");
            }


            if (ModelState.IsValid)
            {

                // Uplaod Photo =======================================================================================================
                string ImageName = Path.GetFileNameWithoutExtension(user.uploadedUserImage.FileName);    // get name
                string ImageExtension = Path.GetExtension(user.uploadedUserImage.FileName);                   // get extension
                ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                user.photo = "~/Uploads/Photos/users/" + ImageName;                                         // Save To DB
                ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/users/"), ImageName);             // Combine To Local Folder
                user.uploadedUserImage.SaveAs(ImageName);                                                       // Move it

                var myUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    first_name = user.first_name,
                    last_name = user.last_name,
                    photo = user.photo,
                    gender = user.gender,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    join_data = DateTime.Now.ToString()
                };
                var result = await UserManager.CreateAsync(myUser, user.PasswordHash);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        // GET: Admin/users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,first_name,last_name,Email,PasswordHash,gender,UserName,PhoneNumber,EmailConfirmed,PhoneNumberConfirmed,uploadedUserImage")] user user)
        {

            ModelState["PasswordHash"].Errors.Clear();  // to remove required Server Side
            if (ModelState.IsValid)
            {
                user.join_data = db.users.Find(user.Id).join_data;
                user.PasswordHash = db.users.Find(user.Id).PasswordHash;
                user.SecurityStamp = db.users.Find(user.Id).SecurityStamp;
                user.AccessFailedCount = db.users.Find(user.Id).AccessFailedCount;
                // Uplaod Photo =======================================================================================================

                if (user.uploadedUserImage != null)
                {
                    string lastPhoto = db.users.Find(user.Id).photo;
                    if (lastPhoto != "~/Images/default-avatar.png")
                    {
                        System.IO.File.Delete(Request.MapPath(lastPhoto));  // delete From Local Folder
                    }

                    string ImageName = Path.GetFileNameWithoutExtension(user.uploadedUserImage.FileName);           // get name
                    string ImageExtension = Path.GetExtension(user.uploadedUserImage.FileName);                     // get extension
                    ImageName = ImageName + '_' + DateTime.Now.ToString("yymmssfff") + ImageExtension;                // Not To Repeat
                    user.photo = "~/Uploads/Photos/users/" + ImageName;                                         // Save To DB
                    ImageName = Path.Combine(Server.MapPath("~/Uploads/Photos/users/"), ImageName);             // Combine To Local Folder
                    user.uploadedUserImage.SaveAs(ImageName);                                                       // Move it
                }
                else
                {
                    user.photo = db.users.Find(user.Id).photo;     // let the last, don't change
                }


                var local = db.Set<user>().Local.FirstOrDefault(f => f.Id == user.Id);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

       
        // POST: Admin/users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            user user = db.users.Find(id);
            if (System.IO.File.Exists(Request.MapPath(user.photo)))
            {
                System.IO.File.Delete(Request.MapPath(user.photo));
            }

            db.users.Remove(user);
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
