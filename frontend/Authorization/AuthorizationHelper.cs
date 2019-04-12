using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using AgendaTech.Business.Entities;
using AgendaTech.View.Models;

namespace AgendaTech.View.Authorization
{
    public class AuthorizationHelper
    {
        public UsuarioLogado ObterUsuarioLogado()
        {   
            var usuarioLogado = new UsuarioLogado();

            if (HttpContext.Current.User is ClaimsPrincipal principal && principal.Identity.IsAuthenticated)
            {
                Claim login = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name") select c).SingleOrDefault();
                usuarioLogado.Login = login.Value;

                Claim guid = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") select c).SingleOrDefault();
                usuarioLogado.uqUsuario = new Guid(guid.Value);

                Claim usuarioEmail = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") select c).SingleOrDefault();
                usuarioLogado.Email = usuarioEmail.Value;

                Claim grupoPrimario = (from c in principal.Claims where c.Type.Equals(@"http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid") select c).SingleOrDefault();
                if (grupoPrimario != null)
                    usuarioLogado.Inscricao = (EnUserType)Enum.Parse(typeof(EnUserType), grupoPrimario.Value);

                Claim idCustomer = (from c in principal.Claims where c.Type.Equals(@"http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid") select c).SingleOrDefault();
                if (idCustomer != null)
                    usuarioLogado.IDCustomer = int.Parse(idCustomer.Value);
                
                Claim usuarioNome = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname") select c).SingleOrDefault();
                usuarioLogado.Nome = usuarioNome.Value;

                HttpContext.Current.Session["SessaoExpirada"] = null;
            }

            return usuarioLogado;
        }
    }
}