using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeSan.HlxPortal.DataCollector
{
    class SiteInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public byte SiteId { get; set; }
        public byte DeviceId { get; set; }

        public override string ToString()
        {
            return string.Format(" SiteId {0}, DeviceId {1}, Address {2}:{3} ", SiteId, DeviceId, Ip, Port);
        }
    }
}
