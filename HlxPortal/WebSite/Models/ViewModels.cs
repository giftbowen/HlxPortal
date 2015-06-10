using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.Common;

namespace LeSan.HlxPortal.WebSite.Models
{
    public class RadiationViewModel
    {
        public List<RadiationDbData> DataList { get; set; }
        public List<string> CameraImageBase64List { get; set; }
    }
}