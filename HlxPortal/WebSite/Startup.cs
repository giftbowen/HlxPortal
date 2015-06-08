using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeSan.HlxPortal.WebSite.Startup))]
namespace LeSan.HlxPortal.WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
