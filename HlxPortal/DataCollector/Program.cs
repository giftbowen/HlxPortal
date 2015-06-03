using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LeSan.HlxPortal.DataCollector
{
    class Program
    {
        private static Socket listener = null;
        static void Main(string[] args)
        {
            string strIP = "";
            int port = 55000;
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

            Thread listenThread = new Thread(ListenClientConnect);
            listenThread.Start();
            listenThread.Join(); 
        }

        private static void ListenClientConnect()
        {
            while(true)
            {
                Socket dataSock = listener.Accept();  // thread blocks here until a connection accepted
                dataSock.ReceiveTimeout = 1000*60;
                Thread collectorThread = new Thread(CollectorThread);
                collectorThread.Start(dataSock);
            }
        }

        private static void CollectorThread(object dataSocket)
        {
            Socket dataSock = (Socket)dataSocket;
            List<Message> msgs = new List<Message>();
            byte siteId;
            byte deviceId;
            while(true)
            {
                try
                {
                    int bufSize = Math.Max(dataSock.Available, 2048);
                    var buf = new byte[bufSize];
                    int received = dataSock.Receive(buf, bufSize, SocketFlags.None);
                    if (received == 0)
                    {
                        SharedTraceSources.Global.TraceEvent(TraceEventType.Error, 0, "Received bytes is 0, closing the connection");
                        break;
                    }
                    msgs.AddRange(Message.ParseMessages(buf, received));
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

                    siteId = msgs[index].Address;
                    deviceId = msgs[index].Declare;
                    msgs.RemoveAll(x => x.IsHeartBeat); // remove heart beat messages, keep the data messages

                    if(deviceId == 1)
                    {
                    //    int bufSize = Math.Max(dataSock.Available, 2048);
                    //var buf = new byte[bufSize];
                    //int received = dataSock.Receive(buf, bufSize, SocketFlags.None);

                        //dataSock.Send() // read radiation flame
                        //dataSock.a
                        var bytesToSend = Message.AssemblyMessage(siteId, Message.DeclareType.RadiationFlame, Message.CmdRadiationFlame);
                        int sent = dataSock.Send(bytesToSend);


                    }

                }
                catch (Exception ex)
                {
                    SharedTraceSources.Global.TraceException(ex, "Unexpected exception caught, closing the connection");
                    break;
                }
            }

            // close the socket and exit the thread.
            dataSock.Close();
        }

//36.        /// <summary>  
//37.        /// 接收消息  
//38.        /// </summary>  
//39.        /// <param name="clientSocket"></param>  
//40.        private static void ReceiveMessage(object clientSocket)  
//41.        {  
//42.            Socket myClientSocket = (Socket)clientSocket;  
//43.            while (true)  
//44.            {  
//45.                try  
//46.                {  
//47.                    //通过clientSocket接收数据  
//48.                    int receiveNumber = myClientSocket.Receive(result);  
//49.                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));  
//50.                }  
//51.                catch(Exception ex)  
//52.                {  
//53.                    Console.WriteLine(ex.Message);  
//54.                    myClientSocket.Shutdown(SocketShutdown.Both);  
//55.                    myClientSocket.Close();  
//56.                    break;  
//57.                }  
//58.            }  
//59.        }  

    }
}
