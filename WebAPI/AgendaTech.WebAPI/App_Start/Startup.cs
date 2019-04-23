using AgendaTech.WebAPI.App_Start;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(Startup))]

namespace AgendaTech.WebAPI.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            var config = new HttpConfiguration();
            
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                  routeTemplate: "api/{controller}/{id}",
                  defaults: new { id = RouteParameter.Optional }
             );
            
            app.UseCors(CorsOptions.AllowAll);

            ActivateAccessTokenGenerator(app);

            app.UseWebApi(config);
        }

        private void ActivateAccessTokenGenerator(IAppBuilder app)
        {
            var tokenConfiguration = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new AccessTokenProvider()
            };
            app.UseOAuthAuthorizationServer(tokenConfiguration);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }  
}
