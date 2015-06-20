using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeSan.HlxPortal.Common;
using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.WebSite.Models;

namespace LeSan.HlxPortal.WebSite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<SiteDbData> allSites = RegularUpdateObjects<List<SiteDbData>>.DefaultObjectInstance;
            return View("SiteMap", allSites);
        }

        public ActionResult UnAuthorized(string msg)
        {
            ViewBag.Msg = msg ?? "您没有权限进行该操作。";

            return View();
        }
        public ActionResult Error(string msg)
        {
            ViewBag.Msg = msg ?? "对不起！该页面无法显示。";

            return View();
        }

        public ActionResult Heatmap()
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            
            DateTime dtStart = DateTime.Now.AddDays(-1); // 24 hours ealier
            var model = HeatmapDataService.GetHeatmapData(connstring, dtStart, DateTime.Now);

            return View(model);
        }
    }
}