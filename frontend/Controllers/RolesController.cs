using System;
using System.Web.Mvc;
using AgendaTech.View.Authorization;
using System.Text;
using AgendaTech.View.Models;
using AgendaTech.Business.Entities;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {        
        private AuthorizationHelper _claimHelper = new AuthorizationHelper();
        private UsuarioLogado _usuarioLogado = new UsuarioLogado();

        public RolesController()
        {         
            _usuarioLogado = _claimHelper.ObterUsuarioLogado();
        }

        [AuthorizeUser(AccessLevel = "/Roles")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = _usuarioLogado.Nome;
            return View();
        }

        public string MenuLateral()
        {
            var cacheResult = HttpContext.Cache.Get(_usuarioLogado.uqUsuario.ToString());
            
            if (cacheResult == null)            
                HttpContext.Cache.Add(_usuarioLogado.uqUsuario.ToString(), null, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);

            return MontarMenu();
        }

        private string MontarMenu()
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
            
            if (_usuarioLogado.Inscricao.Equals(EnUserType.Administrator))
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Customers' title='Clientes'>");
                menu.AppendLine("<span class='item'>Clientes</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Services' title='Serviços'>");
            menu.AppendLine("<span class='item'>Serviços</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            menu.AppendLine("<li>");
            menu.AppendLine("<a href='/Professionals' title='Profissionais'>");
            menu.AppendLine("<span class='item'>Profissionais</span>");
            menu.AppendLine("</a>");
            menu.AppendLine("</li>");

            if (_usuarioLogado.Inscricao.Equals(EnUserType.Administrator) || _usuarioLogado.Inscricao.Equals(EnUserType.Customer))
            {
                menu.AppendLine("<li>");
                menu.AppendLine("<a href='/Users' title='Usuários'>");
                menu.AppendLine("<span class='item'>Usuários</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");
                menu.AppendLine("</li>");
            }

            menu.AppendLine("</ul>");

            menu.AppendLine("<li>");
            menu.AppendLine("<span class='glyphicon glyphicon glyphicon-book'/>");
            menu.AppendLine("<span class='item'>Controles</span>");
            menu.AppendLine("<ul>");
            {
                menu.AppendLine("<li>");
             //   menu.AppendLine("<a href='/Agenda' title='Agenda'>");
                menu.AppendLine("<span class='item'>Agenda</span>");
                menu.AppendLine("</a>");
                menu.AppendLine("</li>");

                menu.AppendLine("<li>");
               // menu.AppendLine("<a href='/MalaDireta' title='Mala Direta'>");
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
                //menu.AppendLine("<a href='/Agendamentos' title='Agendamentos'>");
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
