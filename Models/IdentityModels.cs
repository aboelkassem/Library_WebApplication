using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Library.Models
{
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {

        public string first_name { get; set; } 
        public string last_name { get; set; }
        public string join_data { get; set; }
        public string gender { get; set; }
        public string photo { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser,int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This needs to go before the other rules!

            //modelBuilder.Entity<IdentityUser>().ToTable("users","dbo").HasKey(u => u.Id).Property(u => u.Id).HasColumnType("int");
            modelBuilder.Entity<ApplicationUser>().ToTable("users","dbo").HasKey(u => u.Id).Property(u => u.Id).HasColumnType("int");
            modelBuilder.Entity<CustomUserRole>().ToTable("UserRoles", "dbo").HasKey(r => new { r.RoleId, r.UserId }).Property(r => r.UserId).HasColumnType("int");
            modelBuilder.Entity<CustomUserLogin>().ToTable("UserLogins", "dbo").HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey }).Property(l => l.UserId).HasColumnType("int");
            modelBuilder.Entity<CustomUserClaim>().ToTable("UserClaims", "dbo").Property(c => c.UserId).HasColumnType("int");
            modelBuilder.Entity<CustomRole>().ToTable("Roles", "dbo").HasKey(r => r.Id);


        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}