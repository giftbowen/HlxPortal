using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeSan.HlxPortal.Common;
using LeSan.HlxPortal.WebSite.Models;
using System.Net;
using LeSan.HlxPortal.WebSite;
using LeSan.HlxPortal.WebSite.DataEntity;

namespace LeSan.HlxPortal.WebSite.Controllers
{
    [Authorize]
    [AccessControl]
    public class CpfController : Controller
    {
        // GET: Radiation
        public ActionResult Index(int siteId)
        {
            List<SiteDbData> allSites = RegularUpdateObjects<List<SiteDbData>>.DefaultObjectInstance;
            SiteDbData model = allSites.Where(x => x.SiteId == siteId).First();
            return View("CpfIndex", model);
        }
    }
}