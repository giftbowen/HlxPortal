using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HlxPortal.Startup))]
namespace HlxPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
