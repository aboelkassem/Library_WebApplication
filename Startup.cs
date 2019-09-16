using Library.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(Library.Startup))]
namespace Library
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            //CreateUsers();
        }

        //public void CreateUsers()
        //{
        //    var userManager = new UserManager<ApplicationUser, int>(new UserStore<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>(db));
        //    var user = new ApplicationUser();
        //    userManager.AddToRole(7, "Admins");
        //}

        public void CreateRoles()
        {
            var roleManager = new RoleManager<CustomRole,int>(new CustomRoleStore(db));
   
            if (!roleManager.RoleExists("Admins"))
            {
                CustomRole role = new CustomRole();
                role.Name = "Admins";
                roleManager.Create(role);
            }

        }
    }
}
