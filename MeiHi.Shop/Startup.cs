using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeiHi.Shop.Startup))]
namespace MeiHi.Shop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
