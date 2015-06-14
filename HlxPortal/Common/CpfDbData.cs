using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace LeSan.HlxPortal.Common
{
    [Table(Name="CpfData")]
    public class CpfDbData
    {
        //private DateTime? timeStamp = null;

        ///// <summary>
        ///// Non-column
        ///// </summary>
        //public DateTime TimeStamp
        //{
        //    get { timeStamp = timeStamp ?? Util.FromSN(SN); return timeStamp.Value; }
        //}

        [Column]
        public DateTime Date { get; set; }
        [Column]
        public string SN { get; set; }
        [Column]
        public int SiteId { get; set; }
        [Column]
        public int DeviceId { get; set; }
        [Column]
        public string PlateNumber { get; set; }
        [Column]
        public string VehicleType { get; set; }
        [Column]
        public string Comments { get; set; }
        [Column]
        public string Goods { get; set; }
    }
}
