using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeSan.HlxPortal.Common;
using System.Configuration;
using Newtonsoft.Json;

namespace LeSan.HlxPortal.WebSite.Controllers
{
    [AccessControl]
    [Authorize]
    public class PlcController : Controller
    {
        //
        // GET: /Plc/
        public ActionResult Index(int siteId)
        {
            return RedirectToAction("PlcIndex", new { siteId = siteId });
        }

        public ActionResult PlcIndex(int siteId)
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            PlcDbRecordDataContext db = new PlcDbRecordDataContext(connstring);
            var jsonData = (from record in db.PlcDbRecords where record.SiteId == siteId select record.KeyValueData).First();

            PlcDbData model = JsonConvert.DeserializeObject<PlcDbData>(jsonData);
            return View("PlcIndex", model);
        }
	}
}