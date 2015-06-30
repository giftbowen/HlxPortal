using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeSan.HlxPortal.Common;

namespace LeSan.HlxPortal.DataCollector
{
    public static class SockHelper
    {
        public static bool IsSockTimeout(DateTime lastTimeGetHeartBeat, int timeoutInSeconds = 60)
        {
            return (DateTime.Now - lastTimeGetHeartBeat).TotalSeconds > timeoutInSeconds;
        }

        public static byte[] ReceiveMessage(Socket socket, int maxWaitTimeInSeconds)
        {
            List<byte[]> listBufs = new List<byte[]>();
            List<int> listSize = new List<int>();
            int bufSize = 128;
            int received = 0;
            do
            {
                var buf = new byte[bufSize];
                received = socket.Receive(buf, bufSize, SocketFlags.None);
                listBufs.Add(buf);
                listSize.Add(received);

                // if there's no avaible data now and data not ends with ETX, means there should be more data coming
                if (socket.Available <= 0 && !Message.EndsWithETX(buf, received))
                {
                    for (int i = 1; i <= maxWaitTimeInSeconds; i++)
                    {
                        if (socket.Available > 0)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
            while (socket.Available > 0);

            var stream = new byte[listSize.Sum()];
            int iCursor = 0;
            for (int i = 0; i < listBufs.Count; i++)
            {
                listBufs[i].Take(listSize[i]).ToArray().CopyTo(stream, iCursor);
                iCursor += listSize[i];
            }

            return stream;
        }

        public static List<Message> ReceiveAndParseMessages(Socket socket, byte siteId, Message.DeclareType declare, ref DateTime lastTimeGetHeartBeat, int timeoutInSeconds)
        {
            var buf = ReceiveMessage(socket, timeoutInSeconds);
            var msgs = Message.ParseMessages(buf, buf.Length);
            // update the heart beat receive time and remove the heart beat messages
            if (msgs.Any(x => x.Address == siteId && x.IsHeartBeat))
            {
                lastTimeGetHeartBeat = DateTime.Now;
                msgs.RemoveAll(x => x.IsHeartBeat);
            }
            // remove other messages from the message list
            var otherMsgs = msgs.FindAll(x => x.Address == siteId && x.Declare != (byte)declare);
            if (otherMsgs.Count > 0)
            {
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, string.Format("Received {0} other messages when collecting data {1}, drop them. SiteID {2}.", otherMsgs.Count, declare, siteId));
                msgs.RemoveAll(x => x.Address == siteId && x.Declare != (byte)declare);
            }

            return msgs;
        }

        public static void SendMessage(Socket socket, byte siteId, Message.DeclareType declare, byte[] command)
        {
            var bytesToSend = Message.AssembleMessage(siteId, declare, command);
            int countSent = 0;
            while (countSent != bytesToSend.Length)
            {
                countSent = socket.Send(bytesToSend);
            }
        }

        public static List<Message> SendAndReceiveMsg(Socket socket, byte siteId, Message.DeclareType declare, byte[] command, int numExpected, ref DateTime lastTimeGetHeartBeat, int timeOutInSeconds)
        {
            SendMessage(socket, siteId, declare, command);

            int retry = 0;
            List<Message> msgs = new List<Message>();
            while (retry < 15)
            {
                Thread.Sleep(1000); // sleep a while before every receive, to make sure we get corresponding response message of the request message

                var partialList = ReceiveAndParseMessages(socket, siteId, declare, ref lastTimeGetHeartBeat, timeOutInSeconds);
                msgs.AddRange(partialList);

                if (msgs.Count < numExpected)
                {
                    SharedTraceSources.Global.TraceEvent(TraceEventType.Information, 0, string.Format("Received {0} messages, expect {1}, retrying {2} times. SiteID {3}, Declar {4}.", msgs.Count, numExpected, retry, siteId, declare));
                }
                else
                {
                    if (msgs.Count > numExpected)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Warning, 0, string.Format("Received {0} messages, expect {1}. Take the first {0} ones. SiteID {2}, Declar {3}.", msgs.Count, numExpected, siteId, declare));
                    }
                    return msgs;
                }
            }


            throw new Exception(string.Format("Receive messages failed! Received {0} messages, expect {1}. Have retryed for {2} times abort it. SiteID {3}, Declar {4}.", msgs.Count, numExpected, retry, siteId, declare));
        }

        public static byte[] SendAndReceiveRadiationCameraImage(Socket socket, byte siteId, ref DateTime lastTimeGetHeartBeat)
        {
            SendMessage(socket, siteId, Message.DeclareType.RadiationCamera, Message.CmdRadiationCamera);

            int totalFrames = -1;
            int frameReceived = 0;
            List<Message> frames = new List<Message>();
            var cameraTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutRadiationCameraInSeconds"]);
            while (true)
            {
                var msgs = ReceiveAndParseMessages(socket, siteId, Message.DeclareType.RadiationCamera, ref lastTimeGetHeartBeat, cameraTimeout);

                if (msgs.Count == 0) continue;
                // first time receive camera data, initialize total frame number
                if (totalFrames == -1)
                {
                    totalFrames = msgs[0].Data[0];
                    msgs.RemoveAt(0);
                }

                frames.AddRange(msgs);
                frameReceived += msgs.Count;

                if (frameReceived >= totalFrames)
                {
                    break;
                }
            }

            // validate data
            if (frameReceived != totalFrames)
            {
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, string.Format("When receiving radiation camera data, received {0} frames, 1st frame says there're {1} frames. SiteID {2}. Ignoring this inconsistence and continue the process", frameReceived, totalFrames, siteId));
            }

            if (totalFrames == 0)
            {
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Receive radiation camera iamge faile, data says no frame data, continue to process other datas, there will be no camera data in this record");
                SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Please check the cable connection of radiation camera!");
                return new byte[0];
            }

            int imageLength = frames.Sum(x => x.DataLength);
            byte[] image = new byte[imageLength];
            int iCursor = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].Data.CopyTo(image, iCursor);
                iCursor += frames[i].DataLength;
            }
            
            return image;
        }        
    }
}
