using LeSan.HlxPortal.WebSite.DataEntity;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LeSan.HlxPortal.WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            this.BeginRequest += Application_BeginRequest;
            this.PostAuthorizeRequest += this.Application_PostAuthorizeRequest;

            IdentityManager.InitiBasicUsers();
            CreateRegularUpdateObjects();
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            // nothing
        }

        private void CreateRegularUpdateObjects()
        {
            var task = RegularUpdateObjects<List<SiteDbData>>.Instance.AddDefault(
                TimeSpan.FromMinutes(5),
                () => IdentityManager.UpdateRolesAndGetAllSites());
            task.Wait();
            if (!task.IsCompleted)
            {
                throw new Exception("Loading data source settings failed. Status: " + task.Status);
            }
        }

        void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            PerRequestData.CreateCurrentInstance(HttpContext.Current.Request);
        }
    }
}
