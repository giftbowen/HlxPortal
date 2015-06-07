using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeSan.HlxPortal.DataCollector
{
    public class PlcDbData
    {
        public const int PLC_TOTAL_LENGTH = 27;
        public DateTime TimeStamp { get; set; }
        public byte StopNormal { get; set; }
        public byte Ready { get; set; }
        public byte RadiationPosition { get; set; }
        public byte Radiation { get; set; }
        public byte ShutterClosePosition { get; set; }
        public byte OpenShutterCmd { get; set; }
        public byte RadiationGate { get; set; }
        public byte ShutterOpenPosition { get; set; }
        public byte CollectCmd { get; set; }
        public byte ControlGate { get; set; }
        public byte OpenShutterTimeout { get; set; }
        public byte PressureHigh { get; set; }
        public byte RadiationGateStop { get; set; }
        public byte VehicleFollow { get; set; }
        public byte ShutterFailure { get; set; }
        public byte PressureLow { get; set; }
        public byte VehicleBack { get; set; }
        public byte Loops12Stop { get; set; }
        public byte Loops23Stop { get; set; }
        public byte Loops34Stop { get; set; }
        public byte Authorize { get; set; }
        public byte MainRoomStop { get; set; }
        public byte ManualAuto { get; set; }
        public byte LoopsPhotoelectric { get; set; }
        public byte Photoelectirc1 { get; set; }
        public byte Photoelectirc2 { get; set; }
        public byte Photoelectirc3 { get; set; }
    }
}
