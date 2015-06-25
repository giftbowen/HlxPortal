using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace LeSan.HlxPortal.WebSite
{
    /// <summary>
    /// WebApi Config
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register WebApi
        /// </summary>
        /// <param name="config">WebApi Config</param>
        public static void Register(HttpConfiguration config)
        {
            IAssembliesResolver assembliesResolver = config.Services.GetAssembliesResolver();

            ICollection<Assembly> assemblies = assembliesResolver.GetAssemblies();

            StringBuilder errorsBuilder = new StringBuilder();

            foreach (Assembly assembly in assemblies)
            {
                Type[] exportedTypes = null;
                if (assembly == null || assembly.IsDynamic)
                {
                    // can't call GetExportedTypes on a dynamic assembly
                    continue;
                }

                try
                {
                    exportedTypes = assembly.GetExportedTypes();
                    errorsBuilder.AppendLine("ok webapi register");
                }
                catch (ReflectionTypeLoadException ex)
                {
                    exportedTypes = ex.Types;
                }
                catch (Exception ex)
                {
                    errorsBuilder.AppendLine(ex.ToString());
                }
            }

            if (errorsBuilder.Length > 0)
            {
                //Log errors into Event Log
                Trace.TraceError(errorsBuilder.ToString());
            }

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}