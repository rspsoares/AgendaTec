using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgendaTech.Portal.Startup))]
namespace AgendaTech.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
