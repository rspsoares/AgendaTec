using AgendaTec.Business.Helpers;
using AgendaTec.Portal.App_Start;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AgendaTec.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.ConfigureContainer();
            ProfilesHelper.Initialize();
        }

        void Session_Start(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/Login");
        }
    }
}
