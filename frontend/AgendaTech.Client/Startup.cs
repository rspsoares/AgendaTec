using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgendaTech.Client.Startup))]
namespace AgendaTech.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
