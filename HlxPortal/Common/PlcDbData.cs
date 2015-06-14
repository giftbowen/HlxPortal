using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeSan.HlxPortal.Common
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
        public byte RadiationRoomStop { get; set; }
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

    public class PlcPropertyName
    {
        public static Dictionary<string, string> Dict { get; private set; }

        static PlcPropertyName()
        {
            Dict = new Dictionary<string, string>(){
                {"StopNormal", "急停正常"},
                {"Ready", "就绪"},
                {"RadiationPosition", "源位"},
                {"Radiation", "射线"},
                {"ShutterClosePosition", "快门关位置"},
                {"OpenShutterCmd", "开快门指令"},
                {"RadiationGate", "源室门"},
                {"ShutterOpenPosition", "快门开位置"},
                {"CollectCmd", "采集指令"},
                {"ControlGate", "控制柜门"},
                {"OpenShutterTimeout", "开快门超时"},
                {"PressureHigh", "气压高"},
                {"RadiationRoomStop", "源室急停"},
                {"VehicleFollow", "车辆跟随"},
                {"ShutterFailure", "快门故障"},
                {"PressureLow", "气压低"},
                {"VehicleBack", "车辆逆行"},
                {"Loops12Stop", "地感一二停车"},
                {"Loops23Stop", "地感二三停车"},
                {"Loops34Stop", "地感三四停车"},
                {"Authorize", "授权"},
                {"MainRoomStop", "主控室急停"},
                {"ManualAuto", "手动/自动"},
                {"LoopsPhotoelectric", "地感/光电"},
                {"Photoelectirc1", "光电1"},
                {"Photoelectirc2", "光电2"},
                {"Photoelectirc3", "光电3"},
            };
        }
    }
}
