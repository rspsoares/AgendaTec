using System;
using System.Linq;
using System.Web.Mvc;
using AgendaTech.View.Authorization;
using System.Text;
using AgendaTech.View.Models;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        //IPermissoesFacade _permFacade;
        private AuthorizationHelper claimHelper = new AuthorizationHelper();
        private UsuarioLogado usuarioLogado = new UsuarioLogado();

        public RolesController()//IPermissoesFacade permFacade)
        {
         //   _permFacade = permFacade;
            usuarioLogado = claimHelper.ObterUsuarioLogado(false);
        }

        [AuthorizeUser(AccessLevel = "/Roles")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }

        public string MenuLateral()
        {
            string uqUsuario = string.Empty;
            System.Security.Claims.ClaimsPrincipal principal =  System.Web.HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal;
            if (null != principal)
            {
                System.Security.Claims.Claim usuario = (from c in principal.Claims where c.Type.Equals(@"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") select c).SingleOrDefault<System.Security.Claims.Claim>();
                uqUsuario = usuario.Value;
            }

            var cacheResult = HttpContext.Cache.Get(uqUsuario);
            if (cacheResult == null)
            {
                var resultados ="/Home,/Administracao";
                HttpContext.Cache.Add(uqUsuario, resultados, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
                return MontarMenu(resultados);
            }
            else
                return MontarMenu(cacheResult.ToString());
        }

        private string MontarMenu(string permissoes)
        {
            var menu = new StringBuilder();

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Home' title='Principal'>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-home'/>");
            menu.AppendLine("<span class='item'>Principal</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");
            
            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon-edit'/>");
            menu.AppendLine("<span class='nav-title-item'>Cadastros</span>");
            menu.AppendLine("<ul>");
            
            if (permissoes.Contains("/Administracao"))
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Customers' title='Clientes'>");
                menu.AppendLine("<span class='item'>Clientes</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Servicos' title='Serviços'>");
            menu.AppendLine("<span class='item'>Serviços</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Usuários' title='Usuarios'>");
            menu.AppendLine("<span class='item'>Usuarios</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");
            menu.AppendLine("</li>");            

            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-book'/>");
            menu.AppendLine("<span class='item'>Controles</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Agenda' title='Agenda'>");
                menu.AppendLine("<span class='item'>Agenda</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");

                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/MalaDireta' title='Mala Direta'>");
                menu.AppendLine("<span class='item'>Mala Direta</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("</li>");
            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-th-list'/>");
            menu.AppendLine("<span class='item'>Relatorios</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Agendamentos' title='Agendamentos'>");
                menu.AppendLine("<span class='item'>Agendamentos</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("</li>");
            menu.AppendLine("</ul>");
            menu.AppendLine("</li></ul>");

            return menu.ToString();
        }
    }
}
