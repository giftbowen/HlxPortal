using System;
using System.Collections.Generic;
using LeSan.HlxPortal.DataCollector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeSan.HlxPortal.Common;
using System.Configuration;
using LeSan.HlxPortal.FakeDataProvider;

namespace HlxPortal.Tests
{
    [TestClass]
    public class DbTest
    {
        [TestMethod]
        public void PopulateRadiationData()
        {
            FakeDataProviderProgram.PopulateRadiationData();
        }

        [TestMethod]
        public void PopuldateCpfData()
        {
            FakeDataProviderProgram.PopuldateCpfData();
        }

        [TestMethod]
        public void PopuldatePlcData()
        {
            FakeDataProviderProgram.PopuldatePlcData();
        }
    }
}
