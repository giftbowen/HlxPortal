using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using LeSan.HlxPortal.Common;

namespace LeSan.HlxPortal.WebSite
{
    public class HeatmapDataService
    {
        public static List<HeatmapIndicator> GetHeatmapIndicators(string connectionString)
        {
            DataContext db = new DataContext(connectionString);
            var heatmapIndicatorTable = db.GetTable<HeatmapIndicator>();

            return (from h in heatmapIndicatorTable orderby h.Id select h).ToList();
        }

        public static HeatmapViewModel GetHeatmapData(string connectionString, DateTime startTime, DateTime endTime)
        {
            List<HeatmapIndicator> indicators = GetHeatmapIndicators(connectionString);

            StringBuilder sb = new StringBuilder("Select SiteId");

            foreach(var indi in indicators)
            {
                sb.Append(string.Format(", (Case When Max({0}) > {1} then 1 else 0 end) as {0}", indi.PropertyName, indi.Threshold));
            }

            sb.Append(" From RadiationData Where Date >= @startDate and Date <= @endDate and Timestamp >= @startTime and Timestamp <= @endTime Group By SiteId");
            //sb.Append(" From RadiationData Group By SiteId");
            string query = sb.ToString();

            List<HeatmapViewModel.SiteModel> siteObjs = new List<HeatmapViewModel.SiteModel>();
            List<SiteDbData> allSites = RegularUpdateObjects<List<SiteDbData>>.DefaultObjectInstance;
            Dictionary<int, SiteDbData> dictSites = allSites.ToDictionary(x => (int)x.SiteId);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@startDate", startTime.Date);
                    comm.Parameters.AddWithValue("@endDate", endTime.Date);
                    comm.Parameters.AddWithValue("@startTime", startTime);
                    comm.Parameters.AddWithValue("@endTime", endTime);

                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {

                        var who = reader["SiteId"];
                        var a1 = reader.GetOrdinal("SiteId");
                        var a2 = reader.GetByte(a1);
                        int siteId = (byte)reader["SiteId"];
                        SiteDbData site = dictSites[siteId];
                        HeatmapViewModel.SiteModel siteObj = new HeatmapViewModel.SiteModel()
                        {
                            SiteId = siteId,
                            SiteInfo = site,
                            HeatmapData = new Dictionary<string, int>()
                        };

                        foreach (var indi in indicators)
                        {
                            siteObj.HeatmapData[indi.PropertyName] = (int)reader[indi.PropertyName];
                        }

                        siteObjs.Add(siteObj);
                    }

                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SharedTraceSources.Global.TraceException(ex, "Error occurs when trying query heatmap data from DB");
                throw;
            }

            // if some site doesn't have data in radiationdata table, then add them in the list.
            var siteObjDict = siteObjs.ToDictionary( x => x.SiteId);
            foreach(var site in allSites)
            {
                if (!siteObjDict.ContainsKey(site.SiteId))
                {
                    siteObjs.Add(new HeatmapViewModel.SiteModel() { SiteId = site.SiteId, SiteInfo = site, HeatmapData = null });
                }
            }


            return new HeatmapViewModel() { Indicators = indicators, SiteObjs = siteObjs };
        }
        
    }
}