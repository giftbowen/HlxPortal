using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.WebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LeSan.HlxPortal.WebSite
{
    public class IdentityManager
    {
        public static UserManager<ApplicationUser> AppUserManager { get; private set; }
        public static ApplicationDbContext AppDbContext { get; set; }

        static IdentityManager()
        {
            AppDbContext = new ApplicationDbContext();
            AppUserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(AppDbContext));
        }

        public static List<SiteDbData> UpdateRolesAndGetAllSites()
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var siteTable = db.GetTable<SiteDbData>();
            var siteList = (from site in siteTable select site).ToList();

            UpdateRoles(siteList);
            return siteList;
        }

        public static void UpdateRoles(List<SiteDbData> siteList)
        {
            var siteIdList = siteList.Select(s => s.SiteId.ToString());
            var roleNameList = AppDbContext.Roles.Select(r => r.Name).ToList();
            var roleList = AppDbContext.Roles.ToList();

            foreach (var siteId in siteIdList)
            {
                if (!roleNameList.Contains(siteId))
                {
                    AppDbContext.Roles.Add(new IdentityRole(siteId));
                }
            }

            AppDbContext.SaveChanges();

            foreach (var role in roleList)
            {
                if (role.Name != Consts.RoleAdmin && role.Name != Consts.RoleVip 
                    && !siteIdList.Contains(role.Name))
                {
                    DeleteRole(role.Id);
                }
            }

            AppDbContext.SaveChanges();
        }

        public static void InitiBasicRoles()
        {
            var roleNameList = AppDbContext.Roles.Select(r => r.Name).ToList();
            if (!roleNameList.Contains(Consts.RoleAdmin))
            {
                AppDbContext.Roles.Add(new IdentityRole(Consts.RoleAdmin));
            }
            if (!roleNameList.Contains(Consts.RoleVip))
            {
                AppDbContext.Roles.Add(new IdentityRole(Consts.RoleVip));
            }

            AddUser("admin", "111111");
            AddUser("vip", "111111");
            AddUser("xm1", "111111");

            var user = AppUserManager.FindByName("admin");
            AppUserManager.AddToRole(user.Id, Consts.RoleAdmin);
            user = AppUserManager.FindByName("vip");
            AppUserManager.AddToRole(user.Id, Consts.RoleVip);
            user = AppUserManager.FindByName("xm1");
            AppUserManager.AddToRole(user.Id, "1");
            AppUserManager.AddToRole(user.Id, "3");
            AppUserManager.AddToRole(user.Id, "4");

            AppDbContext.SaveChanges();

            //var role1 = aa.Roles.Add(new IdentityRole("site1"));
            //int a1 = aa.SaveChanges();
            //var u1 = await UserManager.FindAsync("xm1", "111111");
            //var res = UserManager.AddToRole(u1.Id, "site1");
            //var a = UserManager.IsInRole(u1.Id, "site1");
            //return null;
        }

        public static void AddUser(string userName, string passWord)
        {
            var user = new ApplicationUser() { UserName = userName };
            var result = AppUserManager.Create(user, passWord);
        }



        private static void DeleteRole(string roleId)
        {
            var roleUsers = AppDbContext.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
            var role = AppDbContext.Roles.Find(roleId);

            foreach (var user in roleUsers)
            {
                AppUserManager.RemoveFromRole(user.Id, role.Name);
            }

            AppDbContext.Roles.Remove(role);
        }
    }
}