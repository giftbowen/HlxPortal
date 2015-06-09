using Microsoft.AspNet.Identity.EntityFramework;
using LeSan.HlxPortal.WebSite;

namespace LeSan.HlxPortal.WebSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Password { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(Consts.DbConnectionStringName)
        {
        }
    }
}