using System;
using System.Collections.Generic;
using LeSan.HlxPortal.DataCollector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeSan.HlxPortal.Common;

namespace HlxPortal.Tests
{
    [TestClass]
    public class DbTest
    {
        private const string connectionString0 = @"Data Source=(localdb)\Projects;Initial Catalog=HlxPortal;Integrated Security=True;Connect Timeout=30";
        private const string connectionString1 = @"Data Source=Bowenz-1;Initial Catalog=HlxPortal;Integrated Security=True";
        private const string connString = connectionString0;

        [TestMethod]
        public void TestMethod1()
        {
            var radiationData = new RadiationDbData()
            {
                SiteId = 0,
                Date = DateTime.Now.Date,
                TimeStamp = DateTime.Now,
                CameraImage = "0",
                Dose1 = 1,
                Dose2 = 2,
                Dose3 = 3,
                Dose4 = 4,
                Dose5 = 5,
                Flame = 1,
                Gate = 2,
                Humidity = 3,
                Position = 4,
                Shutter = 5,
                Temperature = 6,
            };

            DbHelper.InsertRadiationDbData(connString, radiationData);
        }

        private int Cycle(int a, int delta, int max)
        {
            return (a + delta) % max;
        }

        public void PopulateSiteRadiationData(byte siteId, DateTime dtStart, DateTime dtEnd)
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

                DbHelper.InsertRadiationDbData(connString, radiationData);

                dose1 = Cycle(dose1, 1, 60); dose2 = Cycle(dose2, 1, 60); dose3 = Cycle(dose3, 1, 60); dose4 = Cycle(dose4, 1, 60); dose5 = Cycle(dose5, 1, 60);
                b1 = (byte)Cycle(b1, 1, 2); b2 = (byte)Cycle(b2, 1, 2);

                temperature = (temperature + 0.1f) % 30;
                humidity = (humidity + 0.2f) % 80;
            }
        }

        [TestMethod]
        public void PopulateRadiationData()
        {
            DateTime dtStart = DateTime.Now.Date.AddDays(-1);
            DateTime dtEnd = DateTime.Now;

            PopulateSiteRadiationData(1, dtStart, dtEnd);
            PopulateSiteRadiationData(2, dtStart, dtEnd);
            PopulateSiteRadiationData(4, dtStart, dtEnd);
        }

        public void PopulateSiteCpfData(int siteId, DateTime dtStart, DateTime dtEnd)
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

            DbHelper.InsertCpfDbData(connString, cpfList);
        }

        [TestMethod]
        public void PopuldateCpfData()
        {
            PopulateSiteCpfData(2, DateTime.Now.AddDays(-1), DateTime.Now);
            PopulateSiteCpfData(3, DateTime.Now.AddDays(-1), DateTime.Now);
            PopulateSiteCpfData(4, DateTime.Now.AddDays(-1), DateTime.Now);
        }

        public void PopulateSitePlcData(int siteId, byte value)
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

            DbHelper.InsertPlcData(connString, plcData);
        }

        [TestMethod]
        public void PopuldatePlcData()
        {
            PopulateSitePlcData(1, 0);
            PopulateSitePlcData(2, 1);
        }
    }
}
