using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.WebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using LeSan.HlxPortal.Common;
using System.Diagnostics;

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
            AppUserManager.UserValidator = new UserValidator<ApplicationUser>(AppUserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        /// <summary>
        /// Store site id as Role.RoleName
        /// </summary>
        /// <returns></returns>
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
                if (!siteIdList.Contains(role.Name))
                {
                    DeleteRole(role.Id);
                }
            }

            AppDbContext.SaveChanges();
        }

        public static void InitiBasicUsers()
        {
            AddUser(new UserViewModel()
            {
                UserName = "Admin",
                Password = "111111",
                RoleType = Consts.RoleAdmin,
                Comments = "管理员用户具有最高权限"
            });
            AddUser(new UserViewModel()
            {
                UserName = "Vip",
                Password = "111111",
                RoleType = Consts.RoleVip,
                Comments = "Vip 用户 可访问所有站点"
            });

            AddUser(new UserViewModel()
            {
                UserName = "User1",
                Password = "111111",
                RoleType = Consts.RoleNormal,
                Comments = "用户1可访问站点 1 2 3",
                SiteList = new List<string>() { "1", "2", "3" }
            });
        }

        public static string ValidateRoleType(string roleType)
        {
            if (roleType == Consts.RoleAdmin || roleType == Consts.RoleVip || roleType == Consts.RoleNormal)
            {
                return roleType;
            }
            else
            {
                return Consts.RoleNormal;
            }
        }

        public static IEnumerable<string> AddUser(UserViewModel user)
        {
            ApplicationUser appUser = new ApplicationUser()
            {
                UserName = user.UserName,
                RegisterTime = DateTime.Now,
                RoleType = ValidateRoleType(user.RoleType),
                Comments = user.Comments
            };

            var result = AppUserManager.Create(appUser, user.Password);
            if (!result.Succeeded)
            {
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Create user failed");
                return result.Errors ;
            }
            
            if (user.SiteList != null && user.RoleType != Consts.RoleAdmin && user.RoleType != Consts.RoleVip)
            {
                foreach (var siteId in user.SiteList)
                {
                    var res = AppUserManager.AddToRole(appUser.Id, siteId);
                    if (!res.Succeeded)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, string.Format("Grant user access to site {0} failed", siteId));
                        return result.Errors;
                    }
                }
            }
            
            return null;
        }

        public static string UpdateUser(UserViewModel user)
        {
            var appUser = AppUserManager.FindByName(user.UserName);
            appUser.RoleType = user.RoleType;
            appUser.Comments = user.Comments;

            if (user.RoleType == Consts.RoleAdmin || user.RoleType == Consts.RoleVip)
            {
                user.SiteList = new List<string>();
            }
            
            // role(site) to delete
            var rolesToDelete = appUser.Roles.Where(x => !user.SiteList.Contains(x.Role.Name)).ToList();
            foreach(var role in rolesToDelete)
            {
                AppUserManager.RemoveFromRole(appUser.Id, role.Role.Name);
            }

            var curRoles = appUser.Roles.Select(x => x.Role.Name);
            // role(site) to add
            var rolesToAdd = user.SiteList.Where(x => !curRoles.Contains(x));
            foreach(var role in rolesToAdd)
            {
                AppUserManager.AddToRole(appUser.Id, role);
            }

            AppDbContext.SaveChanges();
            return null;
        }

        public static void DeleteUser(string userName)
        {
            var appUser = AppUserManager.FindByName(userName);

            var rolesToDelete = appUser.Roles.ToList();
            foreach (var role in rolesToDelete)
            {
                AppUserManager.RemoveFromRole(appUser.Id, role.Role.Name);
            }

            AppDbContext.Users.Remove(appUser);

            AppDbContext.SaveChanges();
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

        public static List<ApplicationUser> GetAllUsers()
        {
            return AppDbContext.Users.ToList();
        }

        public static UserViewModel GetUser(string id)
        {
            var user = AppUserManager.FindById(id);
            var model = new UserViewModel()
            {
                Id = id,
                UserName = user.UserName,
                RoleType = user.RoleType,
                RegisterTime = user.RegisterTime,
                Comments = user.Comments,
                SiteList = user.Roles.Select(x => x.Role.Name).ToList()
            };

            return model;
        }
    }
}