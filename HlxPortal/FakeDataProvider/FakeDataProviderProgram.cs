using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeSan.HlxPortal.FakeDataProvider
{
    public class FakeDataProviderProgram
    {
        static void Main(string[] args)
        {
            PopulateRadiationData();
            PopuldateCpfData();
            PopuldatePlcData();
        }

        public static void PopulateRadiationData()
        {
            DateTime dtStart = DateTime.Now.Date.AddDays(-1);
            DateTime dtEnd = DateTime.Now;

            FakeDataGenerator.PopulateSiteRadiationData(1, dtStart, dtEnd);
            FakeDataGenerator.PopulateSiteRadiationData(2, dtStart, dtEnd);
            FakeDataGenerator.PopulateSiteRadiationData(4, dtStart, dtEnd);
        }

        public static void PopuldateCpfData()
        {
            FakeDataGenerator.PopulateSiteCpfData(1, DateTime.Now.AddDays(-1), DateTime.Now);
            FakeDataGenerator.PopulateSiteCpfData(2, DateTime.Now.AddDays(-1), DateTime.Now);
            FakeDataGenerator.PopulateSiteCpfData(3, DateTime.Now.AddDays(-1), DateTime.Now);
            FakeDataGenerator.PopulateSiteCpfData(4, DateTime.Now.AddDays(-1), DateTime.Now);
        }
        public static void PopuldatePlcData()
        {
            FakeDataGenerator.PopulateSitePlcData(1, 0);
            FakeDataGenerator.PopulateSitePlcData(2, 1);
            FakeDataGenerator.PopulateSitePlcData(3, 0);
            FakeDataGenerator.PopulateSitePlcData(4, 1);
        }
    }
}
