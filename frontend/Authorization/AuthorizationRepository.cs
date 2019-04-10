﻿using System;
using System.Linq;
using System.Web;
//using AgendaTech.Permissoes.Application.Contracts;
//using AgendaTech.Permissoes.Application.Bindings;
using AgendaTech.View.Models;
using System.Security.Claims;
using AgendaTech.Business.Entities;

namespace AgendaTech.View.Authorization
{
    public class AuthorizationRepository
    {
     //   IPermissoesFacade _permissoes;
       
        public AuthorizationRepository()
        {
            //_permissoes = (IPermissoesFacade)new PermissoesFacade();
        }

        public string BuscarPermissoes(string uqUsuario, HttpContextBase httpContext)
        {
            var cacheResult = httpContext.Cache.Get(uqUsuario.ToString());
            if(cacheResult == null)
            {
                var resultados = "/Home,/Administracao";
                httpContext.Cache.Add(uqUsuario, resultados, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                return resultados;
            }
            else
              return cacheResult.ToString();            
        }

        public void ObterUsuarioLogado(out string msgErro)
        {
            msgErro = string.Empty;
            UsuarioLogado usuario = new UsuarioLogado();

            try
            {
                ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
                if (null != principal)
                {
                    Claim nome = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name") select c).SingleOrDefault<System.Security.Claims.Claim>();
                    usuario.Login = nome.Value;

                    Claim guid = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") select c).SingleOrDefault<System.Security.Claims.Claim>();
                    usuario.uqUsuario = new Guid(guid.Value);

                    Claim usuarioEmail = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") select c).SingleOrDefault<System.Security.Claims.Claim>();
                    usuario.Email = usuarioEmail.Value;

                    Claim grupoPrimario = (from c in principal.Claims where c.Type.Equals(@"http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid") select c).SingleOrDefault<System.Security.Claims.Claim>();
                    if(grupoPrimario != null)
                        usuario.Inscricao = (EnUserType)Enum.Parse(typeof(EnUserType), grupoPrimario.Value);

                    //Claim grupoSecundario = (from c in principal.Claims where c.Type.Equals(@"http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid") select c).SingleOrDefault<System.Security.Claims.Claim>();
                    //if(grupoSecundario != null)
                    //    usuario.Inscricao = int.Parse(grupoSecundario.Value);
                }
            }
            catch (Exception)
            {
                msgErro = "Erro ao obter usuário logado";
            }
        }
    }
}