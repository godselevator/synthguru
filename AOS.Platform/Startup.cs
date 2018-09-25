using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AOS.Platform.Startup))]
namespace AOS.Platform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
