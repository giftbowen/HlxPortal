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

namespace LeSan.HlxPortal.WebSite.Controllers
{
    [Authorize]
    [AccessControl]
    public class RadiationController : Controller
    {
        // GET: Radiation
        public ActionResult Index(int siteId)
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var radiationTable = db.GetTable<RadiationDbData>();

            DateTime dtStart = DateTime.Now.AddDays(-1).Date; // yesterday

            var radiations = (from r in radiationTable where r.Date >= dtStart && r.SiteId == siteId orderby r.TimeStamp select r).ToList();

            var lastestRadiaions = radiations.Skip(radiations.Count - Consts.NumberRadiationCameraImages).Take(Consts.NumberRadiationCameraImages);

            List<string> cameraBase64s = new List<string>();

            foreach(var r in lastestRadiaions)
            {
                try
                {
                    var path = Util.GetRadiationImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigRadiationCameraRoot], (byte)siteId, r.CameraImage);
                    Image img = Image.FromFile(path);
                    byte[] arr;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        arr = ms.ToArray();
                    }
                    cameraBase64s.Add(Convert.ToBase64String(arr));
                }
                catch(Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex, "Exception caught when loading radiation camera image");
                    cameraBase64s.Add("");
                }
            }

            var model = new RadiationViewModel()
            {
                SiteId = siteId,
                DataList = radiations,
                CameraImageBase64List = cameraBase64s
            };

            return View(model);
        }
    }
}