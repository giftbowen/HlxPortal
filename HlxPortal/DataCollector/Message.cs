using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeSan.HlxPortal.Common;

namespace LeSan.HlxPortal.DataCollector
{
    public class Message
    {
        public enum DeclareType
        {
            /// <summary>
            /// 放射源监控火焰
            /// </summary>
            RadiationFlame = 01,
            /// <summary>
            /// 放射源监控摄像头图像
            /// </summary>
            RadiationCamera = 02,
            /// <summary>
            /// 剂量数据
            /// </summary>
            RadiationDose = 03,
            /// <summary>
            /// CPF图像数据
            /// </summary>
            CPFImage = 04,
            /// <summary>
            /// PLC状态数据
            /// </summary>
            PLCStatus = 05,
            /// <summary>
            /// 温湿度数据
            /// </summary>
            TemperatureHumidity = 06,
            /// <summary>
            /// 快门状态
            /// </summary>
            ShutterStatus = 07,
            /// <summary>
            /// 位置/震动/倾斜
            /// </summary>
            Position = 08,
            /// <summary>
            /// 源室门
            /// </summary>
            RadiationGate = 09,
            /// <summary>
            /// PLC复位命令
            /// </summary>
            PLCReset = 10,
        }
        
        #region Command type
        // Commands:
        /// <summary>
        /// 读放射源监控火焰状态   RH
        /// </summary>
        public static readonly byte[] CmdRadiationFlame = Encoding.ASCII.GetBytes("RH");
        /// <summary>
        /// 读放射源监控摄像头状态 RS
        /// </summary>
        public static readonly byte[] CmdRadiationCamera = Encoding.ASCII.GetBytes("RS");
        /// <summary>
        /// 读剂量数据 RJ
        /// </summary>
        public static readonly byte[] CmdRadiationDose = Encoding.ASCII.GetBytes("RJ");
        /// <summary>
        /// 读CPF图像数据 RC
        /// </summary>
        public static readonly byte[] CmdCPFImage = Encoding.ASCII.GetBytes("RC");
        /// <summary>
        /// 读PLC状态数据 RP
        /// </summary>
        public static readonly byte[] CmdPLCStatus = Encoding.ASCII.GetBytes("RP");
        /// <summary>
        /// 读温湿度数据	RW
        /// </summary>
        public static readonly byte[] CmdTemperatureHumidity = Encoding.ASCII.GetBytes("RW");
        /// <summary>
        /// 读快门状态数据 RK
        /// </summary>
        public static readonly byte[] CmdShutterStatus = Encoding.ASCII.GetBytes("RK");
        /// <summary>
        /// 读位置/震动/倾斜数据	RZ
        /// </summary>
        public static readonly byte[] CmdPosition = Encoding.ASCII.GetBytes("RZ");
        /// <summary>
        /// 读源室门数据	RY
        /// </summary>
        public static readonly byte[] CmdRadiationGate = Encoding.ASCII.GetBytes("RY");
        /// <summary>
        /// PLC复位命令	RL
        /// </summary>
        public static readonly byte[] CmdPLCReset = Encoding.ASCII.GetBytes("RL");
        #endregion

        public static readonly byte[] STX = Encoding.ASCII.GetBytes("###");
        public static readonly byte[] ETX = Encoding.ASCII.GetBytes("***");
        public static readonly byte[] RadiationCameraImageHead = { 0xFF, 0xD8 };
        public static readonly byte[] RadiationCameraImageRear = { 0xFF, 0xD9 };
        
        public const int CMD_LENGTH = 2;
        public const int DATALEN_LENGTH = 2;

        public const int DoseDetectorNumber = 5;
        public const int PerDoseDataLength = 5;

        public const int CPFLengthFactor = 256;

        public byte Address { get; set; }
        public byte Declare { get; set; }
        public UInt16 DataLength { get; set; }

        public byte[] Command { get; set; }

        public byte[] Data { get; set; }

        public bool IsHeartBeat { get; set; }
        public static List<Message> ParseMessages(byte[] buffer, int size)
        {
            byte[] stream = buffer.Take(size).ToArray();
            List<Message> msgs = new List<Message>();

            int iCursor = 0;

            while (true)
            {
                try
                {
                    if ((iCursor = stream.IndexOfPattern(STX, iCursor)) < 0)
                    {
                        break;
                    }
                    iCursor += STX.Length;
                    var msg = new Message();

                    msg.Address = stream[iCursor]; // Address
                    iCursor++;
                    msg.Declare = stream[iCursor]; // Delcare
                    iCursor++;
                    var presumedETX = stream.Skip(iCursor).Take(ETX.Length).ToArray(); // assume we hit ETX here
                    if (presumedETX.SequenceEqual(ETX)) // check if it's ETX or not
                    {
                        msg.IsHeartBeat = true;
                        msgs.Add(msg);
                        iCursor+= ETX.Length;
                        continue;
                    }
                    // not an heart beat, it's a real data message, continue parsing
                    msg.IsHeartBeat = false;
                    var bytesDataLength = stream.Skip(iCursor).Take(DATALEN_LENGTH).ToArray(); // Data length byte array
                    msg.DataLength = BigEndianBitConverter.ToUInt16(bytesDataLength, 0); // Data length uint16
                    iCursor += DATALEN_LENGTH;
                    msg.Command = stream.Skip(iCursor).Take(CMD_LENGTH).ToArray(); // Command, it's not being used in the message sent from client;
                    iCursor += CMD_LENGTH;
                    int realDataLength = msg.DataLength;
                    if (msg.Declare == (byte)DeclareType.CPFImage)
                    {
                        realDataLength *= CPFLengthFactor;
                    }
                    msg.Data = stream.Skip(iCursor).Take(realDataLength).ToArray(); // Data itself
                    iCursor += realDataLength;
                    
                    presumedETX = stream.Skip(iCursor).Take(ETX.Length).ToArray(); // assume we hit ETX here
                    if (!presumedETX.SequenceEqual(ETX)) // check if it's ETX or not
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "ETX not found at the end of a message");
                    }
                    iCursor += ETX.Length;
                    msgs.Add(msg);

                }
                catch(Exception ex)
                {
                    // catch whatever exception occurs here and continue the parsing
                    SharedTraceSources.Global.TraceException(ex);
                }
            }

            return msgs;
        }

        public byte[] Assemble()
        {
            var buffer = new byte[1024];
            int iCursor = 0;
            STX.CopyTo(buffer, iCursor); // STX
            iCursor += STX.Length;
            buffer[iCursor] = Address; // Address
            iCursor++;
            buffer[iCursor] = Declare; // Declare
            iCursor++;
            var dataLen = BigEndianBitConverter.GetBytes(DataLength); // get data length byte array
            dataLen.CopyTo(buffer, iCursor); // Data length
            iCursor += dataLen.Length;
            Command.CopyTo(buffer, iCursor); // Comand
            iCursor += Command.Length;
            Data.CopyTo(buffer, iCursor); // Data
            iCursor += DataLength;
            ETX.CopyTo(buffer, iCursor); // ETX
            iCursor += ETX.Length;
            var res = buffer.Take(iCursor).ToArray();
            return res;
        }

        public static byte[] AssembleMessage(byte address, DeclareType declare, byte[] command)
        {
            var msg = new Message()
            {
                Address = address,
                Declare = (byte)declare,
                DataLength = 0,
                Command = command,
                Data = new byte[0],
            };

            return msg.Assemble();
        }

        public static byte[] AssembleCPFMessage(byte address)
        {
            return AssembleMessage(address, DeclareType.CPFImage, CmdCPFImage);
        }

        public static byte[] AssemblePLCMessage(byte address)
        {
            return AssembleMessage(address, DeclareType.PLCStatus, CmdPLCStatus);
        }

        public bool ValidateDataLength(int expectedLength)
        {
            if (this.DataLength != expectedLength)
            {
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, string.Format("Data length of {0} is {1}, expecte {2}, when collecting data, drop this round and start a new one"
                    , this.Declare, this.DataLength, expectedLength));
                return false;
            }

            return true;
        }

        public static bool EndsWithETX(byte[] buf, int size)
        {
            if (size >= ETX.Length)
            {
                var presumedETX = buf.Skip(size - ETX.Length).Take(ETX.Length);
                if (presumedETX.SequenceEqual(ETX))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
