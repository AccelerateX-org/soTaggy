using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoTaggy.Web.Startup))]
namespace SoTaggy.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
