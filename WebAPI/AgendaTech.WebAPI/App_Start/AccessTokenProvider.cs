using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgendaTech.WebAPI.App_Start
{
    public class AccessTokenProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserFacade _userFacade;

        public AccessTokenProvider()
        {
            _userFacade = new UserFacade();
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {            
            if (_userFacade.VerifyPassword(context.UserName, context.Password, out UserAccountDTO userAccount))
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", "user"));
                context.Validated(identity);
            }
            else
            {
                context.SetError("Acesso inválido", "As credenciais do usuário não conferem!");
                return;
            }            
        }
    }
}