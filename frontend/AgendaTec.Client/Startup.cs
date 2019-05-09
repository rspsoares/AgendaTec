using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgendaTec.Client.Startup))]
namespace AgendaTec.Client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
