using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace LeSan.HlxPortal.WebSite.DataEntity
{
    [Table]
    public class HeatmapIndicator
    {
        [Column]
        public int Id { get; set; }
        [Column]
        public string PropertyName { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Threshold { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public string Green { get; set; }
        [Column]
        public string Red { get; set; }
    }
}