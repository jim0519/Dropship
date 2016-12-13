using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dropship.Startup))]
namespace Dropship
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
