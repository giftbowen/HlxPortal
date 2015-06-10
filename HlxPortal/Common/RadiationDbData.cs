using System;
using System.Data.Linq.Mapping;

namespace LeSan.HlxPortal.Common
{
    [Table(Name="RadiationData")]
    public class RadiationDbData
    {
        [Column]
        public DateTime Date { get; set; }
        [Column]
        public DateTime TimeStamp { get; set; }
        [Column]
        public byte SiteId { get; set; }
        [Column]
        public byte Flame { get; set; }
        [Column]
        public byte Shutter { get; set; }
        [Column]
        public byte Position { get; set; }
        [Column]
        public byte Gate { get; set; }
        [Column]
        public float Temperature { get; set; }
        [Column]
        public float Humidity { get; set; }
        [Column]
        public string CameraImage { get; set; }
        [Column]
        public float Dose1 { get; set; }
        [Column]
        public float Dose2 { get; set; }
        [Column]
        public float Dose3 { get; set; }
        [Column]
        public float Dose4 { get; set; }
        [Column]
        public float Dose5 { get; set; }
    }

    public class DoseData
    {
        public byte ID { get; set; }
        public float Data { get; set; }
    }
}
