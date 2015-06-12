using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}