using System.Collections.Generic;
using System.Web.Mvc;
using AgendaTech.View.Authorization;
using Newtonsoft.Json;
using AgendaTech.View.Models;

namespace AgendaTech.View.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AuthorizationHelper claimHelper = new AuthorizationHelper();
        private UsuarioLogado usuarioLogado = new UsuarioLogado();
     
        public HomeController()
        {            
            usuarioLogado = claimHelper.ObterUsuarioLogado();            
        }

        [AuthorizeUser(AccessLevel = "/Home")]
        public ActionResult Index()
        {
            ViewBag.NomeUsuario = usuarioLogado.Nome;
            return View();
        }        
    }
}
