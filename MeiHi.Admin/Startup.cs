using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeiHi.Admin.Startup))]
namespace MeiHi.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
