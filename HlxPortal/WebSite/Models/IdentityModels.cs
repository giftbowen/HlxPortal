using Microsoft.AspNet.Identity.EntityFramework;

namespace LeSan.HlxPortal.WebSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public static string ConnStriongName = "HlxPortal0";
        //public static string ConnStriongName = "HlxPortal1";
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ApplicationUser.ConnStriongName)
        {
        }
    }
}