using System;
using System.Collections.Generic;
using LeSan.HlxPortal.DataCollector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HlxPortal.Tests
{
    [TestClass]
    public class DbTest
    {
        private const string connectionString0 = @"Data Source=(localdb)\Projects;Initial Catalog=HlxPortal;Integrated Security=True;Connect Timeout=30";
        private const string connectionString1 = @"Data Source=Bowenz-1;Initial Catalog=HlxPortal;Integrated Security=True";
        private const string connString = connectionString1;

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

        [TestMethod]
        public void TestMethod2()
        {
            var cpfList = new List<CpfDbData>();
            for (int i = 0; i < 10; i++)
            {
                var cpfData = new CpfDbData()
                {
                    Date = DateTime.Now.Date,
                    SiteId = 10000 + i,
                    SN ="2015 test sn",
                    DeviceId = 5,
                    PlateNumber = "test plate number",
                    VehicleType = "test vehicle type",
                    Comments = "test comments",
                    Goods = "test goods"
                };
                cpfList.Add(cpfData);
            }

            DbHelper.InsertCpfDbData(connString, cpfList);
        }


    }
}
