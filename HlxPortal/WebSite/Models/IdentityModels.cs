using Microsoft.AspNet.Identity.EntityFramework;
using LeSan.HlxPortal.WebSite;
using System;

namespace LeSan.HlxPortal.WebSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Password { get; set; }
        public DateTime RegisterTime { get; set; }
        public string RoleType { get; set; }
        public string Comments { get; set; } 
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(Consts.DbConnectionStringName)
        {
        }
    }
}