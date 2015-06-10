using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.WebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeSan.HlxPortal.WebSite
{
    /// <summary>
    /// The per-request data class
    /// </summary>
    public class PerRequestData
    {
        public ApplicationUser AppUser { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }
        public ApplicationDbContext DbContext { get; set; }
        public List<SiteDbData> UserSites { get; private set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PerRequestData" /> class from being created
        /// </summary>
        private PerRequestData()
        {
            this.DbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.DbContext));
        }

        /// <summary>
        /// Get the current instance of per-request data
        /// </summary>
        public static PerRequestData Current
        {
            get
            {
                if (HttpContext.Current.Items[Consts.AllRequests] == null)
                {
                    throw new InvalidOperationException("PerRequestData can be used in request handling only, and CreateCurrentInstance() should be called in current request before. ");
                }

                return (PerRequestData)HttpContext.Current.Items[Consts.AllRequests];
            }
        }

        /// <summary>
        /// Create the current instance at the beginning of BeginRequest handler. 
        /// </summary>
        /// <param name="request">The current http request instance.</param>
        public static void CreateCurrentInstance(HttpRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException("This function should be called in a request. ");
            }

            PerRequestData data = new PerRequestData();
            HttpContext.Current.Items[Consts.AllRequests] = data;

            var userId = HttpContext.Current.User.Identity.GetUserId();

            //HttpContext.Current.RewritePath("Account/Login");
            //return;

            //data.AppUser = data.UserManager.FindById(userId);
            data.AppUser = data.UserManager.FindByName("admin");
            data.SetUserSites();

            
        }

        private void SetUserSites()
        {
            List<SiteDbData> allSites = RegularUpdateObjects<List<SiteDbData>>.DefaultObjectInstance;

            if (UserManager.IsInRole(AppUser.Id, Consts.RoleAdmin) || UserManager.IsInRole(AppUser.Id, Consts.RoleAdmin))
            {
                this.UserSites = allSites;
            }
            else
            {
                this.UserSites = (from site in allSites where UserManager.IsInRole(AppUser.Id, site.SiteId.ToString()) select site).ToList();
            }
        }

    }
}