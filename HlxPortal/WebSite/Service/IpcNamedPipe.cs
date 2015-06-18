using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Pipes;
using LeSan.HlxPortal.Common;
using System.Diagnostics;

namespace LeSan.HlxPortal.WebSite
{
    public class IpcNamedPipe
    {
        private static NamedPipeClientStream pipeClient = null;

        public static void Init()
        {
            try
            {
                NamedPipeClientStream pipeClient =
                    new NamedPipeClientStream(".", CommonConsts.IPCPipeName,
                        PipeDirection.InOut, PipeOptions.None);

                SharedTraceSources.Global.TraceEvent(TraceEventType.Information, 0, "HlxPortal website Ipc named pipe is connecting to datacollector(server)...");
                pipeClient.Connect(1000);
            }
            catch (Exception ex)
            {
                SharedTraceSources.Global.TraceException(ex, "Init class IpcNamedPipe failed!");
                // swallow the exception
            }
        }

        public static string SendDataResetPlc(int siteId)
        {
            try
            {
                if (!pipeClient.IsConnected)
                {
                    pipeClient.Connect(1000);
                }
                StreamString ss = new StreamString(pipeClient);
                ss.WriteString(siteId.ToString());
                return "PLC复位 发送成功";
            }
            catch (Exception ex)
            {
                SharedTraceSources.Global.TraceException(ex);
                return "PLC复位 发送失败，请检查 DataCollector 是否正常运行！";
            }
        }
    }
}