using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeSan.HlxPortal.Common;
using LeSan.HlxPortal.WebSite.DataEntity;

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
            ViewBag.Msg = msg ?? "您没有权限进行该操作！";

            return View();
        }
        public ActionResult Error(string msg)
        {
            ViewBag.Msg = msg ?? "对不起！该页面无法显示";

            return View();
        }

        public ActionResult HeatMap()
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var radiationTable = db.GetTable<RadiationDbData>();

            DateTime dtStart = DateTime.Now.AddDays(-1); // 24 hours ealier

            var radiations = (from r in radiationTable where r.Date >= dtStart.Date && r.TimeStamp >= dtStart select r).ToList();

            //select siteid, (case when max(dose1) >= 59 then 1 else 0 end ) as dose1,  from RadiationData where date >= '2015-06-09 00:00:00.000' and timestamp >= '2015-06-09 01:05:00.000' group by siteid
            return View();
        }
    }
}