using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeSan.HlxPortal.WebSite.DataEntity;
using LeSan.HlxPortal.Common;
using System.ComponentModel.DataAnnotations;

namespace LeSan.HlxPortal.WebSite.Models
{
    public class RadiationViewModel
    {
        public SiteDbData Site { get; set; }
        public List<RadiationDbData> DataList { get; set; }
        public List<string> CameraImageBase64List { get; set; }
        public List<string> CameraImageTimeStamp { get; set; }
    }

    public class HeatmapViewModel
    {
        public class SiteModel
        {
            public int SiteId { get; set; }
            public SiteDbData SiteInfo { get; set; }
            public Dictionary<string, int> HeatmapData { get; set; }
        }

        public List<SiteModel> SiteObjs { get; set; }
        public List<HeatmapIndicator> Indicators { get; set; }
    }

    public class CpfViewModel
    {
        public CpfDbData DbData { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Base64CpfImage { get; set; }
        public string Base64LpnImage { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "1111 The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public DateTime RegisterTime { get; set; }

        public string RoleType { get; set; }

        public List<string> SiteList { get; set; }

        public string Comments { get; set; } 
    }
}