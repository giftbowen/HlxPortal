using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace LeSan.HlxPortal.WebSite.DataEntity
{
    [Table(Name = "Site")]
    public class SiteDbData
    {
        [Column]
        public byte SiteId { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public string Location { get; set; }
        [Column]
        public string Description { get; set; }
    }
}