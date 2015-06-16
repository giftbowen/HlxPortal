using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeSan.HlxPortal.WebSite
{
    public class Consts
    {
        /// <summary>
        /// Data which are available for all requests. 
        /// </summary>
        public const string AllRequests = "AllRequests";

        public const string RoleAdmin = "Admin";

        public const string RoleVip = "Vip";

        public const string RoleNormal = "Normal";

        public const int NumberRadiationCameraImages = 3;

        public const string ConfigRadiationCameraRoot = "RadiationCameraRoot";

        public const string ConfigCpfLpnRoot = "CpfLpnRoot";

        public const string DbConnectionStringName = "HlxPortal0";
        //public const string DbConnectionStringName = "HlxPortal1";
    }
}