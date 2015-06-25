using System.Web.Http;

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
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}