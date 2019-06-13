using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_Seguranca.Startup))]
namespace MVC_Seguranca
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
