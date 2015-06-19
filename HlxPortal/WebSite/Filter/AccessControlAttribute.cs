using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using LeSan.HlxPortal.WebSite.Controllers;

namespace LeSan.HlxPortal.WebSite
{
    /// <summary>
    /// Separated access control, only do RBAC, no authorization integrated
    /// </summary>
    public class AccessControlAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated
                && filterContext.ActionParameters.ContainsKey("siteId"))
            {
                int siteId = (int)filterContext.ActionParameters["siteId"];

                var userSites = PerRequestData.Current.UserSites.Select(x => x.SiteId);
                if (!userSites.Contains((byte)siteId))
                {
                    filterContext.Result = new RedirectResult("~/Home/UnAuthorized?msg=您没有访问该站点的权限。");
                }
            }

            // if it's admin controller, then user's role type must be admin
            var adminController = filterContext.Controller as AdminController;
            if (adminController != null)
            {
                if (PerRequestData.Current.AppUser.RoleType != Consts.RoleAdmin)
                {
                    filterContext.Result = new RedirectResult("~/Home/UnAuthorized?msg=您没有权限访问该资源。");
                }
            }
        }
    }

    /// <summary>
    /// Integrated authorize attribute, normal authorize and RBAC
    /// </summary>
    public class HlxAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.User.Identity.IsAuthenticated
                && filterContext.HttpContext.Request.Params.AllKeys.Contains("siteId"))
            {
                string site = filterContext.HttpContext.Request.Params.Get("siteId");
                int siteId = int.Parse(site);

                var userSites = PerRequestData.Current.UserSites.Select(x => x.SiteId);
                if (!userSites.Contains((byte)siteId))
                {
                    filterContext.Result = new RedirectResult("~/account/manage");
                }
            }
        }
    }
}