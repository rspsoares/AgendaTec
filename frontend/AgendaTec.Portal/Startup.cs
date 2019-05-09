using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgendaTec.Portal.Startup))]
namespace AgendaTec.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
