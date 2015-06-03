using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace LeSan.HlxPortal.DataCollector
{
    /// <summary>
    /// Warpper the trace functionality to add trace time, caller info and request ID in each line of trace. 
    /// </summary>
    public static class SharedTraceSources
    {
        /// <summary>
        /// The global trace source for cross class sharing.
        /// </summary>  
        private static readonly TraceSource globalTraceSource = new TraceSource("Global");

        /// <summary>
        /// Random object to generate correlation ID per web request
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Label for In exception, to ensure the TraceException will not be called recursively. 
        /// </summary>
        [ThreadStatic]
        private static bool inException = false;

        /// <summary>
        /// Global Trace source
        /// </summary>
        public static TraceSource Global
        {
            get
            {
                return SharedTraceSources.globalTraceSource;
            }
        }

        /// <summary>
        /// Extension method to write exception to trace source. 
        /// </summary>
        /// <param name="traceSource">The trace source to write trace into</param>
        /// <param name="ex">Exception to trace</param>
        /// <param name="message">The message to trace</param>
        public static void TraceException(this TraceSource traceSource, Exception ex, string message = "")
        {
            if (inException)
            {
                return;
            }

            inException = true;

            try
            {
                traceSource.TraceEvent(TraceEventType.Error, 0, FormatException(ex, message));
            }
            finally
            {
                inException = false;
            }
        }

        /// <summary>
        /// /// Returns a string about current request for trace
        /// </summary>
        /// <param name="traceSource">trace Source</param>
        /// <param name="context">Current context</param>
        //public static void TraceEndRequest(this TraceSource traceSource, HttpContext context)
        //{
        //    if (context == null)
        //    {
        //        traceSource.TraceEvent(TraceEventType.Warning, 0x0004f80f /* tag_abp6p */, "The HttpContext in trace parameter is invalid (null). ");
        //        return; 
        //    }

        //    StringBuilder sb = new StringBuilder();

        //     Request Information
        //     NOTE: that this log is consumed by the usage analysis service, which depends on the log output format. 
        //     If there is any update for existing lines, please make sure there is no break for the usage analysis. 
        //    if (null != HttpContext.Current.User && HttpContext.Current.User.Identity != null &&
        //        HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User is UnimatrixPrincipal)
        //    {
        //        sb.AppendLine("User: " + (HttpContext.Current.User as UnimatrixPrincipal).WindowsAccountName);
        //        sb.AppendLine("User groups: " + string.Join(",", (HttpContext.Current.User as UnimatrixPrincipal).Groups));
        //    }

        //    sb.AppendLine("Client IP: " + HttpContext.Current.Request.UserHostAddress);
        //    sb.AppendLine("Client browser: " + HttpContext.Current.Request.Browser.Browser + HttpContext.Current.Request.Browser.Version);
        //    sb.AppendLine("Request method: " + HttpContext.Current.Request.HttpMethod);
        //    sb.AppendLine("Request path: " + HttpContext.Current.Request.Url.PathAndQuery);

        //    // Response Information
        //    sb.AppendLine("Response status code: " + HttpContext.Current.Response.StatusCode);
        //    sb.AppendLine("Response redirect location: " + HttpContext.Current.Response.RedirectLocation);

        //    traceSource.TraceEvent(TraceEventType.Start, 0x0004f810 /* tag_abp6q */, sb.ToString());
        //}

        /// <summary>
        /// Gets a new correlation ID.
        /// </summary>
        /// <returns>The new correlation ID</returns>
        public static uint NewCorrelationId()
        {
            byte[] bytes = new byte[4];

            lock (random)
            {
                random.NextBytes(bytes);
            }

            uint corId = BitConverter.ToUInt32(bytes, 0);

            return corId;
        }

        #region private variables & methods

        /// <summary>
        /// Trace exception details.
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">The message for the exception</param>
        /// <returns>Formatted exception string</returns>
        private static string FormatException(Exception ex, string message)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "----- [Unimatrix Exception] -----";
            }

            sb.AppendLine(message);

            if (ex != null)
            {
                sb.AppendLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException message: " + ex.InnerException.ToString());
                }
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// Gets a unique ID identifying the current request, will be traced through to any WS calls on the page
        /// The value is retained in a cookie (if web context) or in an equivalent temporary store (if non-web).
        /// </summary>
        /// <returns>The correlationID</returns>
        private static uint GetRequestId()
        {
            //if (HttpContext.Current == null)
            //{
            //    return 0;
            //}

            //uint reqId = 0;

            //object idObject = HttpContext.Current.Items[RequestKeys.RequestID];
            //if (idObject != null && idObject is uint)
            //{
            //    reqId = (uint)idObject;
            //}
            //else
            //{
            //    reqId = NewCorrelationId();
            //    HttpContext.Current.Items[RequestKeys.RequestID] = reqId;
            //}

            //return reqId;
            return 0;
        }
        
        #endregion
    }
}
