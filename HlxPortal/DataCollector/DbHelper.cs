using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeSan.HlxPortal.Common;


namespace LeSan.HlxPortal.DataCollector
{
    public class DbHelper
    {
        public static void InsertRadiationDbData(string connectionString, RadiationDbData radiationData)
        {
            string insert = "INSERT INTO dbo.RadiationData (Date, Timestamp, SiteId, Flame, Shutter, Position, Gate, Temperature, Humidity, CameraImage, Dose1, Dose2, Dose3, Dose4, Dose5)"
            + "VALUES (@Date, @Timestamp, @SiteId, @Flame, @Shutter, @Position, @Gate, @Temperature, @Humidity, @CameraImage, @Dose1, @Dose2, @Dose3, @Dose4, @Dose5)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand comm = new SqlCommand(insert, conn))
            {
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@Date", radiationData.Date);
                comm.Parameters.AddWithValue("@Timestamp", radiationData.TimeStamp);
                comm.Parameters.AddWithValue("@SiteId", radiationData.SiteId);
                comm.Parameters.AddWithValue("@Flame", radiationData.Flame);
                comm.Parameters.AddWithValue("@Shutter", radiationData.Shutter);
                comm.Parameters.AddWithValue("@Position", radiationData.Position);
                comm.Parameters.AddWithValue("@Gate", radiationData.Gate);
                comm.Parameters.AddWithValue("@Temperature", radiationData.Temperature);
                comm.Parameters.AddWithValue("@Humidity", radiationData.Humidity);
                comm.Parameters.AddWithValue("@CameraImage", radiationData.CameraImage);
                comm.Parameters.AddWithValue("@Dose1", radiationData.Dose1);
                comm.Parameters.AddWithValue("@Dose2", radiationData.Dose2);
                comm.Parameters.AddWithValue("@Dose3", radiationData.Dose3);
                comm.Parameters.AddWithValue("@Dose4", radiationData.Dose4);
                comm.Parameters.AddWithValue("@Dose5", radiationData.Dose5);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void InsertCpfDbData(string connectionString, List<CpfDbData> listCpfData)
        {
            DataTable cpfTable = new DataTable();
            cpfTable.Columns.Add("Date", typeof(DateTime));
            cpfTable.Columns.Add("SN", typeof(string));
            cpfTable.Columns.Add("SiteId", typeof(int));
            cpfTable.Columns.Add("DeviceId", typeof(int));
            cpfTable.Columns.Add("PlateNumber", typeof(string));
            cpfTable.Columns.Add("VehicleType", typeof(string));
            cpfTable.Columns.Add("Comments", typeof(string));
            cpfTable.Columns.Add("Goods", typeof(string));

            foreach(var cpfData in listCpfData)
            {
                var row = cpfTable.NewRow();
                row["Date"] = cpfData.Date;
                row["SN"] = cpfData.SN;
                row["SiteId"] = cpfData.SiteId;
                row["DeviceId"] = cpfData.DeviceId;
                row["PlateNumber"] = cpfData.PlateNumber;
                row["VehicleType"] = cpfData.VehicleType;
                row["Comments"] = cpfData.Comments;
                row["Goods"] = cpfData.Goods;
                cpfTable.Rows.Add(row);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.Default))
            {
                bulkCopy.DestinationTableName = "CpfData";
                bulkCopy.BatchSize = listCpfData.Count;
                bulkCopy.BulkCopyTimeout = 60 * 5; // in seconds

                foreach (DataColumn column in cpfTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                bulkCopy.WriteToServer(cpfTable);
            }
        }
    }
}
