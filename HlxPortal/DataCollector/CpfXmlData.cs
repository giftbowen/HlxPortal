using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeSan.HlxPortal.DataCollector
{
    [Serializable]
    public class CpfXmlData
    {	   
        public int SID { get; set; }
        public int DID { get; set; }
        public string SN { get; set; }
        public string CPF { get; set; }
        public string LPN { get; set; }
        public string CN { get; set; }
        public string VT { get; set; }
        public string S { get; set; }
        public string G { get; set; }
    }

    [XmlRoot(ElementName = "HLX", Namespace = "")]    
    [Serializable]
    public class CpfXmlRoot
    {
        [XmlElement("ROW")]
        public List<CpfXmlData> ROWs { get; set; }
    }
}
