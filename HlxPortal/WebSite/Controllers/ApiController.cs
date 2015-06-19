using LeSan.HlxPortal.Common;
using LeSan.HlxPortal.WebSite.Models;
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

    [Authorize]
    public class CpfApiController : ApiController
    {
        public List<CpfViewModel> Get(int siteId, DateTime startDate, DateTime endDate)
        {
            var connstring = ConfigurationManager.ConnectionStrings[Consts.DbConnectionStringName].ConnectionString;
            DataContext db = new DataContext(connstring);
            var cpfTable = db.GetTable<CpfDbData>();

            var cpfDatas = (from c in cpfTable where c.Date >= startDate.Date && c.Date <= endDate.Date && c.SiteId == siteId orderby c.SN select c).ToList();

            List<CpfViewModel> model = new List<CpfViewModel>();

            foreach(var cpf in cpfDatas)
            {
                var cpfModel = new CpfViewModel()
                {
                    DbData = cpf,
                    TimeStamp = Util.FromSN(cpf.SN),
                    Base64CpfImage = "",
                    Base64LpnImage = ""
                };

                //var cpfPath = Util.GetCpfImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigCpfLpnRoot], (byte)siteId, cpfModel.TimeStamp);
                //var lpnPath = Util.GetLpnImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigCpfLpnRoot], (byte)siteId, cpfModel.TimeStamp);

                //cpfPath = @"C:\SD\Github\HlxPortal\HlxPortal\WebSite\images\loading.jpg";
                //lpnPath = @"c:\temp\CpfLpn\003\2015\06\13\20150613000222_003_LPN.jpg";
                
                //cpfModel.Base64CpfImage = Util.LoadJpgAsBase64(cpfPath);
                //cpfModel.Base64LpnImage = Util.LoadJpgAsBase64(lpnPath);

                model.Add(cpfModel);
            }

            return model;
        }
    }

    [Authorize]
    public class CpfImageApiController : ApiController
    {
        public CpfViewModel Get(int siteId, string sn)
        {
            DateTime timestamp = Util.FromSN(sn);
            var cpfPath = Util.GetCpfImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigCpfLpnRoot], (byte)siteId, timestamp);
            var lpnPath = Util.GetLpnImagePath((string)ConfigurationManager.AppSettings[Consts.ConfigCpfLpnRoot], (byte)siteId, timestamp);

            cpfPath = @"c:\temp\CpfLpn\003\2015\06\13\20150613000222_003_CPF.jpg";
            lpnPath = @"c:\temp\CpfLpn\003\2015\06\13\20150613000222_003_LPN.jpg";

            var image = new CpfViewModel()
            {
                DbData = null,
                TimeStamp = timestamp,
                Base64CpfImage = Util.LoadJpgAsBase64(cpfPath),
                Base64LpnImage = Util.LoadJpgAsBase64(lpnPath)
            };

            return image;
        }
    }

    [Authorize]
    public class PlcResetApiController : ApiController
    {
        public string Get(int siteId)
        {
            return IpcNamedPipe.SendDataResetPlc(siteId);
        }
    }

    [Authorize]
    public class DeleteUserApiController : ApiController
    {
        public string Get(string userName)
        {
            if (PerRequestData.Current.AppUser.RoleType != Consts.RoleAdmin)
            {
                return "Not admin, can not delete user!";
            }

            if (userName.ToLowerInvariant() == "Admin")
            {
                return "Can not delete default admin user!";
            }

            IdentityManager.DeleteUser(userName);
            return "ok";
        }
    }
}