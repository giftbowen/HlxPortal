using LeSan.HlxPortal.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace LeSan.HlxPortal.WebSite.Controllers
{
    [Authorize]
    public class RadiationApiController : ApiController
    {
        public List<RadiationDbData> Get(int siteId, DateTime startDate, DateTime endDate)
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var radiationTable = db.GetTable<RadiationDbData>();

            var radiations = (from r in radiationTable where r.Date >= startDate.Date && r.Date <= endDate.Date && r.SiteId == siteId orderby r.TimeStamp select r).ToList();

            return radiations;
        }
    }
}