using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using LeSan.HlxPortal.Common;
using LeSan.HlxPortal.DataCollector;

namespace LeSan.HlxPortal.FakeDataProvider
{
    public class FakeDataGenerator
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["HlxPortal"].ConnectionString;

        private static int Cycle(int a, int delta, int max)
        {
            return (a + delta) % max;
        }

        public static void PopulateSiteRadiationData(byte siteId, DateTime dtStart, DateTime dtEnd)
        {
            int dose1 = 10, dose2 = 20, dose3 = 30, dose4 = 40, dose5 = 50;
            byte b1 = 0, b2 = 1;
            float temperature = 5f;
            float humidity = 30f;
            for (var dt = dtStart; dt < dtEnd; dt += TimeSpan.FromMinutes(5))
            {
                var radiationData = new RadiationDbData()
                {
                    SiteId = siteId,
                    Date = dt.Date,
                    TimeStamp = dt,
                    CameraImage = "0",
                    Dose1 = dose1,
                    Dose2 = dose2,
                    Dose3 = dose3,
                    Dose4 = dose4,
                    Dose5 = dose5,
                    Flame = b1,
                    Gate = b2,
                    Position = b1,
                    Shutter = b2,
                    Humidity = humidity,
                    Temperature = temperature,
                };

                DbHelper.InsertRadiationDbData(ConnectionString, radiationData);

                dose1 = Cycle(dose1, 1, 60); dose2 = Cycle(dose2, 1, 60); dose3 = Cycle(dose3, 1, 60); dose4 = Cycle(dose4, 1, 60); dose5 = Cycle(dose5, 1, 60);
                b1 = (byte)Cycle(b1, 1, 2); b2 = (byte)Cycle(b2, 1, 2);

                temperature = (temperature + 0.1f) % 30;
                humidity = (humidity + 0.2f) % 80;
            }
        }

        public static void PopulateSiteCpfData(int siteId, DateTime dtStart, DateTime dtEnd)
        {
            var cpfList = new List<CpfDbData>();
            for (var dt = dtStart; dt < dtEnd; dt += TimeSpan.FromMinutes(5))
            {
                var cpfData = new CpfDbData()
                {
                    Date = dt.Date,
                    SiteId = siteId,
                    SN = Util.GetSN(dt),
                    DeviceId = 5,
                    PlateNumber = "京A45678挂 " + (dt - dtStart).TotalMinutes,
                    VehicleType = "货车 " + (dt - dtStart).TotalMinutes,
                    Comments = "货物合格" + (dt - dtStart).TotalMinutes,
                    Goods = "各种货物" + (dt - dtStart).TotalMinutes
                };
                cpfList.Add(cpfData);
            }

            DbHelper.InsertCpfDbData(ConnectionString, cpfList);
        }

        public static void PopulateSitePlcData(int siteId, byte value)
        {
            var plcData = new PlcDbData()
            {
                SiteId = siteId,
                TimeStamp = DateTime.Now,
                StopNormal = value,
                Ready = value,
                RadiationPosition = value,
                Radiation = value,
                ShutterClosePosition = value,
                OpenShutterCmd = value,
                RadiationGate = value,
                ShutterOpenPosition = value,
                CollectCmd = value,
                ControlGate = value,
                OpenShutterTimeout = value,
                PressureHigh = value,
                RadiationRoomStop = value,
                VehicleFollow = value,
                ShutterFailure = value,
                PressureLow = value,
                VehicleBack = value,
                Loops12Stop = value,
                Loops23Stop = value,
                Loops34Stop = value,
                Authorize = value,
                MainRoomStop = value,
                ManualAuto = value,
                LoopsPhotoelectric = value,
                Photoelectirc1 = value,
                Photoelectirc2 = value,
                Photoelectirc3 = value,
            };

            DbHelper.InsertPlcData(ConnectionString, plcData);
        }
    }
}
