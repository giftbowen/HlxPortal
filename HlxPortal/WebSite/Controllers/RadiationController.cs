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
    public class RadiationController : Controller
    {
        // GET: Radiation
        public ActionResult Index(int siteId)
        {
            return RedirectToAction("RadiationIndex", new { siteId = siteId });
        }
        public ActionResult RadiationIndex(int siteId)
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var radiationTable = db.GetTable<RadiationDbData>();

            DateTime dtStart = DateTime.Now.AddDays(-1).Date; // yesterday

            var radiations = (from r in radiationTable where r.Date >= dtStart && r.SiteId == siteId orderby r.TimeStamp select r).ToList();

            List<string> lastestImageSN = null;
            if (radiations.Count <= Consts.NumberRadiationCameraImages)
            {
                lastestImageSN = radiations.Select(x => x.CameraImage).ToList();
            }
            else
            {
                lastestImageSN = radiations.Skip(radiations.Count - Consts.NumberRadiationCameraImages).Take(Consts.NumberRadiationCameraImages).Select(x => x.CameraImage).ToList();
            }
            

            List<string> cameraBase64s = new List<string>();
            List<string> timeStamps = new List<string>();

            foreach (var sn in lastestImageSN)
            {
                try
                {
                    var path = Util.GetRadiationImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigRadiationCameraRoot], (byte)siteId, sn);
                    //var path = @"C:\temp\RadiationCamera\003\2015\06\13\20150613000222_003_RC.jpg";
                    var image = Util.LoadJpgAsBase64(path);
                    cameraBase64s.Add(image);
                    var timeStamp = Util.FromSN(sn).ToString("yyyy-MM-dd HH:mm");
                    //var timeStamp = Util.FromSN("20150613000222").ToString("yyyy-MM-dd HH:mm");
                    timeStamps.Add(timeStamp);
                }
                catch(Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex, "Exception caught when loading radiation camera image");
                    cameraBase64s.Add("");
                }
            }

            List<SiteDbData> allSites = RegularUpdateObjects<List<SiteDbData>>.DefaultObjectInstance;

            var model = new RadiationViewModel()
            {
                Site = allSites.Where(s => s.SiteId == siteId).First(),
                DataList = radiations,
                CameraImageBase64List = cameraBase64s,
                CameraImageTimeStamp = timeStamps
            };

            return View(model);
        }
    }
}