using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Drawing;
using System.Drawing.Imaging;
using LeSan.HlxPortal.Common;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace LeSan.HlxPortal.DataCollector
{
    class Program
    {
        private static Socket listener = null;
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["HlxPortal"].ConnectionString;

        private static object lockObj = new object();
        private static List<byte> sitesToResetPlc = new List<byte>();

        static void Main(string[] args)
        {
            Thread ipcThread = new Thread(ServerThread);
            ipcThread.Start();

            string strIP = ConfigurationManager.AppSettings["Ip"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            IPAddress ip = null;
            if (string.IsNullOrWhiteSpace(strIP)){
                ip = IPAddress.Any;
            }else{
                ip = IPAddress.Parse(strIP);
            }
            
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(ip, port));
            listener.Listen(100);   // 100 connection requests in queue
            //trace begin listen
            SharedTraceSources.Global.TraceInformation("Begin listening");

            while (true)
            {
                Socket dataSock = listener.Accept();  // thread blocks here until a connection accepted
                dataSock.ReceiveTimeout = 1000 * 60 * 1000;
                Thread collectorThread = new Thread(CollectorThread);
                collectorThread.Start(dataSock);
            }
        }

        private static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer = null;
            while (true)
            {
                pipeServer = new NamedPipeServerStream(CommonConsts.IPCPipeName, PipeDirection.InOut, 1);
                // Wait for a client to connect
                pipeServer.WaitForConnection();

                SharedTraceSources.Global.TraceEvent(TraceEventType.Information, 0, "HlxPortal website has connected to datacollector.");
                try
                {
                    while (true)
                    {
                        // Read the request from the client.
                        StreamString ss = new StreamString(pipeServer);
                        string site = ss.ReadString();
                        byte siteId;
                        if (byte.TryParse(site, out siteId))
                        {
                            lock (lockObj)
                            {
                                sitesToResetPlc.Add(siteId);
                            }
                        }
                        else
                        {
                            SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "DataCollector Ipc received incorrect site id from website :" + site);
                        }
                    }
                }
                // Catch the IOException that is raised if the pipe is broken or disconnected. 
                catch (IOException ex)
                {
                    SharedTraceSources.Global.TraceException(ex);
                    // swallow and continue
                }
                catch(Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex);
                    break;
                }
            }
            
            pipeServer.Close();
        }


        private static void CollectorThread(object dataSocket)
        {
            Socket dataSock = (Socket)dataSocket;
            List<Message> msgs = new List<Message>();
            byte siteId = 0;
            byte deviceId = 0;
            bool errorOccured = false;
            DateTime lastTimeGetHeartBeat = DateTime.Now;

            while (true)
            {
                try
                {
                    if (SockHelper.IsSockTimeout(lastTimeGetHeartBeat))
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Timeout when receiving heart beat, closing the connection");
                        errorOccured = true;
                        break;
                    }

                    byte[] stream = SockHelper.ReceiveMessage(dataSock);

                    byte[] fake1 = new byte[] { 0x36, 0x35, 0x34, 0x33, 0x32, 0x31, 0x0D, 0x0A };
                    byte[] fake2 = new byte[] { 0x23, 0x23, 0x23, 0x04, 0x01, 0x2A, 0x2A, 0x2A };
                    if (stream.IndexOfPattern(fake2) != -1)
                    {
                        Thread.Sleep(TimeSpan.FromDays(1));
                    }

                    msgs.AddRange(Message.ParseMessages(stream, stream.Length));

                    if (stream.IndexOfPattern(fake1) != -1)
                    {
                        msgs.Add(new Message() { IsHeartBeat = true, Address = 3, Declare = 1 });
                    }
                    if (msgs.Count == 0)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "No message parsed out of the received bytes");
                        continue;
                    }

                    // we care only the heart beat message now
                    int index = msgs.FindIndex(x => x.IsHeartBeat);
                    if (index < 0)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Warning, 0, "The first messages receive contains no hear beat message, cann't get device id, try again to receive heart beat message");
                        continue;
                    }
                    lastTimeGetHeartBeat = DateTime.Now;

                    siteId = msgs[index].Address;
                    deviceId = msgs[index].Declare;
                    msgs.RemoveAll(x => x.IsHeartBeat); // remove heart beat messages, keep the data messages
                    SharedTraceSources.Global.TraceEvent(TraceEventType.Information, 0, string.Format("heart beat received with siteId {0}, deviceId {1}", siteId, deviceId));
                    break;
                }
                catch (Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex, "Exception caught when get heart beat message, closing the connection");
                    errorOccured = true;
                    break;
                }
            }

            // close the socket and exit the thread.
            if (errorOccured)
            {
                dataSock.Close();
                return;
            }

            // site id and device id parsed, begin the collection process
            while (true)
            {
                if (SockHelper.IsSockTimeout(lastTimeGetHeartBeat))
                {
                    // it's the only way to exit the collecting loop
                    SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Timeout after the last heart beat received, closing the connection");
                    break;
                }
                int intervalRadiation = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalCollectRadiationDataInMinutes"]) * 1000;
                int intervalPlc = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalCollectPlcDataInMinutes"]) * 1000;
                intervalRadiation = intervalPlc = 1;
                try
                {
                    switch (deviceId)
                    {
                        case 1: CollectRadiationData(dataSock, siteId, ref lastTimeGetHeartBeat); Thread.Sleep(intervalRadiation); break;
                        case 2: CollectCPFData(dataSock, siteId, ref lastTimeGetHeartBeat); break;
                        case 3: CollectPLCData(dataSock, siteId, ref lastTimeGetHeartBeat); Thread.Sleep(intervalPlc); break;
                        default:
                            SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Invalid device Id:" + deviceId + " . Closing the connection");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    // catch any exception here, trace it and continue the process until the heart beat times out.
                    SharedTraceSources.Global.TraceException(ex, "Swallow the exception and continue the process until heart beat times out");

                    switch(deviceId)
                    {
                        case 1: Thread.Sleep(intervalRadiation); break;
                        case 3: Thread.Sleep(intervalPlc); break;
                    }
                }
            }
            dataSock.Close();
        }

        public static void CollectCPFData(Socket dataSock, byte siteId, ref DateTime lastTimeGetHeartBeat)
        {
            // in the case of CPF image, data will auto sent to sever whenever there's a new record.
            var msgs = SockHelper.ReceiveAndParseMessages(dataSock, siteId, Message.DeclareType.CPFImage, ref lastTimeGetHeartBeat);

            List<CpfXmlData> cpfXmlList = new List<CpfXmlData>();
            foreach (var msg in msgs)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    using (MemoryStream ms = new MemoryStream(msg.Data))
                    {
                        doc.Load(ms);
                        XmlSerializer xmlserilize = new XmlSerializer(typeof(CpfXmlDataList));
                        var records = (CpfXmlDataList)xmlserilize.Deserialize(ms);
                        cpfXmlList.AddRange(records.ROW);
                    }
                }catch(Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex, "Error when deserialize cpf xml file, drop this record and continue to process the next");
                }
            }

            List<CpfDbData> cpfList = new List<CpfDbData>();
            // process each cpf record
            foreach (var record in cpfXmlList)
            {
                DateTime dtSN = Util.FromSN(record.SN);
                var cpf = new CpfDbData()
                {
                    Date = dtSN.Date,
                    SiteId = siteId,
                    DeviceId = record.DID,
                    SN = record.SN,
                    PlateNumber = record.CN,
                    VehicleType = record.VT,
                    Comments = record.S,
                    Goods = record.G
                };
                // save cpf image and plate image to local
                string cpfLpnRoot = ConfigurationManager.AppSettings["CpfLpnRoot"];
                byte[] bitmap = Convert.FromBase64String(record.CPF);
                using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                {
                    // read configuration of root
                    string path = Util.GetCpfImagePath(cpfLpnRoot, siteId, dtSN);
                    Util.CreateDirectoryOfFilePath(path);
                    image.Save(path, ImageFormat.Jpeg);
                }
                bitmap = Convert.FromBase64String(record.LPN);
                using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                {
                    // read configuration of root 
                    string path = Util.GetLpnImagePath(cpfLpnRoot, siteId, dtSN);
                    Util.CreateDirectoryOfFilePath(path);
                    image.Save(path, ImageFormat.Jpeg);
                }

                cpfList.Add(cpf);
            }

            // save cpf db data into db
            DbHelper.InsertCpfDbData(ConnectionString, cpfList);
        }

        public static void CollectPLCData(Socket dataSock, byte siteId, ref DateTime lastTimeGetHeartBeat)
        {
            // try get if there's pending reset plc command to send
            bool resetPlc = false;
            lock(lockObj)
            {
                if (sitesToResetPlc.Any(x => x == siteId))
                {
                    resetPlc = true;
                    sitesToResetPlc.RemoveAll(x => x == siteId);
                }
            }
            if (resetPlc)
            {
                SockHelper.SendMessage(dataSock, siteId, Message.DeclareType.PLCReset, Message.CmdPLCReset);
            }

            Message msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.PLCStatus, Message.CmdPLCStatus, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(PlcDbData.PLC_TOTAL_LENGTH))
            {
                return;
            }

            var plcData = new PlcDbData()
            {
                SiteId = siteId,
                TimeStamp = DateTime.Now,
                StopNormal = msg.Data[0],
                Ready = msg.Data[1],
                RadiationPosition = msg.Data[2],
                Radiation = msg.Data[3],
                ShutterClosePosition = msg.Data[4],
                OpenShutterCmd = msg.Data[5],
                RadiationGate = msg.Data[6],
                ShutterOpenPosition = msg.Data[7],
                CollectCmd = msg.Data[8],
                ControlGate = msg.Data[9],
                OpenShutterTimeout = msg.Data[10],
                PressureHigh = msg.Data[11],
                RadiationRoomStop = msg.Data[12],
                VehicleFollow = msg.Data[13],
                ShutterFailure = msg.Data[14],
                PressureLow = msg.Data[15],
                VehicleBack = msg.Data[16],
                Loops12Stop = msg.Data[17],
                Loops23Stop = msg.Data[18],
                Loops34Stop = msg.Data[19],
                Authorize = msg.Data[20],
                MainRoomStop = msg.Data[21],
                ManualAuto = msg.Data[22],
                LoopsPhotoelectric = msg.Data[23],
                Photoelectirc1 = msg.Data[24],
                Photoelectirc2 = msg.Data[25],
                Photoelectirc3 = msg.Data[26],
            };

            DbHelper.InsertPlcData(ConnectionString, plcData);
        }

        public static void CollectRadiationData(Socket dataSock, byte siteId, ref DateTime lastTimeGetHeartBeat)
        {
            var radiationData = new RadiationDbData() { SiteId = siteId, TimeStamp = DateTime.Now };
            radiationData.Date = radiationData.TimeStamp.Date;

            // radiation flame
            Message msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.RadiationFlame, Message.CmdRadiationFlame, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(1))
            {
                return;// continue;
            }
            radiationData.Flame = msg.Data[0];

            // radiation temperature and humidity
            msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.TemperatureHumidity, Message.CmdTemperatureHumidity, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(4))
            {
                return;// continue;
            }

            radiationData.Humidity = BigEndianBitConverter.ToUInt16(msg.Data, 0) / 10.0F;
            radiationData.Temperature = BigEndianBitConverter.ToUInt16(msg.Data, 2) / 10.0F;

            // radiatoin shutter status
            msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.ShutterStatus, Message.CmdShutterStatus, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(1))
            {
                return;// continue;
            }
            radiationData.Shutter = msg.Data[0];

            // radiation positioin status
            msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.Position, Message.CmdPosition, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(1))
            {
                return;// continue;
            }
            radiationData.Position = msg.Data[0];

            // radiaton gate data
            msg = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.RadiationGate, Message.CmdRadiationGate, 1, ref lastTimeGetHeartBeat)[0];
            if (!msg.ValidateDataLength(1))
            {
                return;// continue;
            }
            radiationData.Gate = msg.Data[0];

            // radiaton dose
            var msgs = SockHelper.SendAndReceiveMsg(dataSock, siteId, Message.DeclareType.RadiationDose, Message.CmdRadiationDose, Message.DoseDetectorNumber, ref lastTimeGetHeartBeat);
            // parse dose data
            List<DoseData> doseList = new List<DoseData>();
            for (int i = 0; i < Message.DoseDetectorNumber; i++ )
            {
                if (!msgs[i].ValidateDataLength(Message.PerDoseDataLength)) // 5 bytes. first byte is dose detector id, the other 4 is dose data
                {
                    return;
                }

                var dose = new DoseData()
                {
                    ID = msgs[i].Data[0],
                    Data = BigEndianBitConverter.ToUInt32(msgs[i].Data, 1) * 25.0F / 151.0F, // formula: data*25/151, uSv/h
                };
                doseList.Add(dose);
            }

            var sortedList = doseList.OrderBy(x => x.ID);
            radiationData.Dose1 = sortedList.ElementAt(0).Data;
            radiationData.Dose2 = sortedList.ElementAt(1).Data;
            radiationData.Dose3 = sortedList.ElementAt(2).Data;
            radiationData.Dose4 = sortedList.ElementAt(3).Data;
            radiationData.Dose5 = sortedList.ElementAt(4).Data;

            string cameraImageSN = "";
            try
            {
                // radiation camera image
                byte[] bitmap = SockHelper.SendAndReceiveRadiationCameraImage(dataSock, siteId, ref lastTimeGetHeartBeat);

                using (Image image = Image.FromStream(new MemoryStream(bitmap)))
                {
                    string path = Util.GetRadiationImagePath(ConfigurationManager.AppSettings["RadiationCameraRoot"], siteId, radiationData.TimeStamp);
                    Util.CreateDirectoryOfFilePath(path);
                    image.Save(path, ImageFormat.Jpeg); 
                }

                cameraImageSN = Util.GetSN(radiationData.TimeStamp);
            }catch(Exception ex)
            {
                SharedTraceSources.Global.TraceException(ex, "Save camera image failed for the record of " + Util.GetSN(radiationData.TimeStamp));
            }

            radiationData.CameraImage = cameraImageSN;

            DbHelper.InsertRadiationDbData(ConnectionString, radiationData);
        }
    }
}
